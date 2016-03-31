using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace barf.lib.Interface.Data
{
	public interface IDataHelper
	{
		SqlCommand SetCommand(string commandValue, bool isDefault = true);

		void AddInputParameter(string parameterName, object parameterValue, SqlCommand command);

		void AddOutputParameter(string parameterName, SqlCommand command);

		string GetValue(SqlCommand command);

		IDataReader ExecuteReader(SqlCommand command);

		int ExecuteNonQuery(SqlCommand command);

		object GetScalarValue(SqlCommand command);

		int GetInt32ReturnValue(string parameterName, SqlCommand command);

		void Close(IDataReader reader);

		void Close(SqlCommand command);

		string GetInitialCatalog(string database);

		List<KeyValuePair<string, string>> GetDatabaseSelections();
	}
}
