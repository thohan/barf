using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Web;
using barf.lib.Interface.Data;

namespace barf.lib.Data
{
	internal class DataHelper : IDataHelper
	{
		public string GetInitialCatalog(string database)
		{
			var list = GetDatabaseSelections();
			var initialCatalog = list.Count == 0 ? string.Empty : list[0].Value;	// Set this to the initial guy by default. THIS IS IMPORTANT!

			foreach (var pair in list)
			{
				if (pair.Key == database)
				{
					initialCatalog = pair.Value;
					break;
				}
			}

			return initialCatalog;
		}

		public List<KeyValuePair<string, string>> GetDatabaseSelections()
		{
			var separator = new[] { "," };
			var list = new List<KeyValuePair<string, string>>();

			if (ConfigurationManager.AppSettings["cmsDatabases"] != null)
			{
				var listarray = ConfigurationManager.AppSettings["cmsDatabases"].Split(separator, StringSplitOptions.None);

				foreach (var pair in listarray)
				{
					var colon = new[] {":"};
					var vals = pair.Split(colon, StringSplitOptions.None);
					var db = new KeyValuePair<string, string>(vals[0], vals[1]);
					list.Add(db);
				}
			}

			return list;
		}

		public string ConnectionString
		{
			get
			{
				HttpContext.Current.Session["DCMS_Connection"] = ConfigurationManager.ConnectionStrings["DCMS_Connection"] != null ? ConfigurationManager.ConnectionStrings["DCMS_Connection"].ConnectionString : string.Empty;

				// This initialCatalog thing is only used by the admin. A regular website using the content admin won't have this key.
				// It won't have an initial catalog of "myCmsDatabase" either.
				var initialCatalog = HttpContext.Current.Session["InitialCatalog"] == null
									? GetInitialCatalog(string.Empty)
									: HttpContext.Current.Session["InitialCatalog"].ToString();

				if (!string.IsNullOrWhiteSpace(initialCatalog))
				{
					HttpContext.Current.Session["DCMS_Connection"] = HttpContext.Current.Session["DCMS_Connection"].ToString().Replace("myCmsDatabase", initialCatalog);
				}

				return HttpContext.Current.Session["DCMS_Connection"].ToString();
			}
			set
			{
				HttpContext.Current.Session["DCMS_Connection"] = value;
			}
		}

		public string CentralConnectionString
		{
			get
			{
				HttpContext.Current.Session["NewaysSites_Connection"] = ConfigurationManager.ConnectionStrings["NewaysSites"] != null
																		? ConfigurationManager.ConnectionStrings["NewaysSites"].ConnectionString
																		: string.Empty;

				return HttpContext.Current.Session["NewaysSites_Connection"].ToString();
			}
			set
			{
				HttpContext.Current.Session["NewaysSites_Connection"] = value;
			}
		}

		public SqlCommand SetCommand(string commandValue, bool isDefault = true)
		{
			var connection = isDefault ? new SqlConnection(ConnectionString) : new SqlConnection(CentralConnectionString);
			var command = new SqlCommand(commandValue, connection);
			command.CommandType = CommandType.StoredProcedure;
			command.Connection = connection;
			return command;
		}

		public void AddInputParameter(string parameterName, object parameterValue, SqlCommand command)
		{
			command.Parameters.AddWithValue("@" + parameterName, parameterValue);
		}

		internal static void AddStringOutputParameter(string parameterName, SqlCommand command)
		{
			command.Parameters.Add("@" + parameterName, SqlDbType.NVarChar, 500);
			command.Parameters["@" + parameterName].Direction = ParameterDirection.Output;
		}

		public void AddOutputParameter(string parameterName, SqlCommand command)
		{
			command.Parameters.Add("@" + parameterName, SqlDbType.Int, 4);
			command.Parameters["@" + parameterName].Direction = ParameterDirection.Output;
		}

		internal static void AddInputOutputParameter(string parameterName, object parameterValue, SqlCommand command)
		{
			command.Parameters.AddWithValue("@" + parameterName, parameterValue);
			command.Parameters["@" + parameterName].Direction = ParameterDirection.InputOutput;
		}

		internal static void AddReturnParameter(string parameterName, SqlCommand command)
		{
			command.Parameters.Add("@" + parameterName, SqlDbType.Int, 4);
			command.Parameters["@" + parameterName].Direction = ParameterDirection.ReturnValue;
		}

		public int GetInt32ReturnValue(string parameterName, SqlCommand command)
		{
			if (command.Parameters["@" + parameterName].Value.ToString().Length == 0) return 0;

			return int.Parse(command.Parameters["@" + parameterName].Value.ToString(), CultureInfo.InvariantCulture);
		}

		internal static long GetInt64ReturnValue(string parameterName, SqlCommand command)
		{
			if (command.Parameters["@" + parameterName].Value.ToString().Length == 0) return 0;

			return long.Parse(command.Parameters["@" + parameterName].Value.ToString(), CultureInfo.InvariantCulture);
		}

		internal static decimal GetDecimalReturnValue(string parameterName, SqlCommand command)
		{
			if (command.Parameters["@" + parameterName].Value.ToString().Length == 0) return 0;

			return decimal.Parse(command.Parameters["@" + parameterName].Value.ToString(), CultureInfo.InvariantCulture);
		}

		internal static string GetStringReturnValue(string parameterName, SqlCommand command)
		{
			if (command.Parameters["@" + parameterName].Value.ToString().Length == 0) return string.Empty;

			return command.Parameters["@" + parameterName].Value.ToString();
		}

		internal static byte GetByteReturnValue(string parameterName, SqlCommand command)
		{
			if (string.IsNullOrWhiteSpace(parameterName))
			{
				return 0;
			}

			return byte.Parse(command.Parameters["@" + parameterName].Value.ToString(), CultureInfo.InvariantCulture);
		}

		public IDataReader ExecuteReader(SqlCommand command)
		{
			try
			{
				using (command)
				{
					command.Connection.Open();
					return command.ExecuteReader();
				}
			}
			catch
			{
				Close(command);
				throw;
			}
		}

		public int ExecuteNonQuery(SqlCommand command)
		{
			try
			{
				using (command)
				{
					command.Connection.Open();
					return command.ExecuteNonQuery();
				}
			}
			catch
			{
				Close(command);
				throw;
			}
		}

		public int ExecuteNonQuery(string sql)
		{
			var connection = new SqlConnection(ConnectionString);
			var command = new SqlCommand(sql, connection);
			command.CommandType = CommandType.Text;
			command.Connection = connection;
			return ExecuteNonQuery(command);
		}

		public int ExecuteNonQuery(string sql, int commandTimeout)
		{
			var sqlConnection = new SqlConnection(ConnectionString);

			var sqlCommand = new SqlCommand(sql, sqlConnection)
			{
				CommandType = CommandType.Text,
				Connection = sqlConnection,
				CommandTimeout = commandTimeout
			};

			return ExecuteNonQuery(sqlCommand);
		}

		public DataTable GetDataTable(SqlCommand command)
		{
			try
			{
				using (command)
				{
					var dataSet = new DataSet();
					var adapter = new SqlDataAdapter(command);
					adapter.Fill(dataSet);
					adapter.Dispose();
					return dataSet.Tables[0];
				}
			}
			finally
			{
				Close(command);
			}
		}

		public DataSet GetDataSet(SqlCommand command)
		{
			try
			{
				using (command)
				{
					var dataSet = new DataSet();
					var adapter = new SqlDataAdapter(command);
					adapter.Fill(dataSet);
					adapter.Dispose();
					return dataSet;
				}
			}
			finally
			{
				Close(command);
			}
		}

		public string GetValue(string sql)
		{
			var returnVal = String.Empty;
			var dataSet = GetDataSet(sql);

			foreach (DataRow row in dataSet.Tables[0].Rows)
			{
				returnVal = row[0].ToString();
				break;
			}

			return returnVal;
		}

		public string GetValue(string sql, int commandTimeout)
		{
			var returnVal = string.Empty;
			var ds = GetDataSet(sql, commandTimeout);

			foreach (DataRow row in ds.Tables[0].Rows)
			{
				returnVal = row[0].ToString();
				break;
			}

			return returnVal;
		}

		public string GetValue(SqlCommand command)
		{
			var results = string.Empty;
			var reader = ExecuteReader(command);

			if (reader.Read())
			{
				results = reader[0].ToString();
			}

			Close(reader);
			Close(command);
			return results;
		}

		public object GetScalarValue(SqlCommand command)
		{
			try
			{
				using (command)
				{
					if (command.Connection.State == ConnectionState.Closed)
					{
						command.Connection.Open();
					}

					return command.ExecuteScalar();
				}
			}
			finally
			{
				Close(command);
			}
		}

		public DataSet GetDataSet(string sql)
		{
			var sqlConnection = new SqlConnection(ConnectionString);

			var sqlCommand = new SqlCommand(sql, sqlConnection)
			{
				CommandType = CommandType.Text,
				Connection = sqlConnection
			};

			return GetDataSet(sqlCommand);
		}

		public DataSet GetDataSet(string sql, int commandTimeout)
		{
			var sqlConnection = new SqlConnection(ConnectionString);

			var sqlCommand = new SqlCommand(sql, sqlConnection)
			{
				CommandType = CommandType.Text,
				Connection = sqlConnection,
				CommandTimeout = commandTimeout
			};

			return GetDataSet(sqlCommand);
		}

		public void Close(IDataReader reader)
		{
			if (reader != null)
			{
				reader.Close();
				reader.Dispose();
			}
		}

		public void Close(SqlCommand command)
		{
			command.Connection.Close();
			command.Dispose();
		}
	}
}
