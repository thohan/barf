using System;
using System.Collections.Generic;
using barf.lib.Interface.Data;
using barf.lib.Model;

namespace barf.lib.Data
{
	public class UtilityDao : IUtilityDao
	{
		private readonly IDataHelper dataHelper;

		public UtilityDao(IDataHelper dataHelper)
		{
			this.dataHelper = dataHelper;
		}

		public string GetInitialCatalog(string database)
		{
			return dataHelper.GetInitialCatalog(database);
		}

		public List<KeyValuePair<string, string>> GetDatabaseSelections()
		{
			return dataHelper.GetDatabaseSelections();
		}

		public List<KeyValuePair<string, string>> GetAdminUserDatabases(string username)
		{
			var results = new List<KeyValuePair<string, string>>();

			var command = dataHelper.SetCommand("csp_user_get_databases", false);
			dataHelper.AddInputParameter("Username", username, command);
			var reader = dataHelper.ExecuteReader(command);

			while (reader.Read())
			{
				var pair = new KeyValuePair<string, string>(reader["DatabaseID"].ToString(), reader["DatabaseName"].ToString());
				results.Add(pair);
			}

			dataHelper.Close(reader);
			dataHelper.Close(command);
			return results;
		}

		public int GetAdminUserRoleID(string username)
		{
			var result = 0;
			var command = dataHelper.SetCommand("csp_user_get_admin_role_id", false);
			dataHelper.AddInputParameter("Username", username, command);
			var reader = dataHelper.ExecuteReader(command);

			if (reader.Read())
			{
				result = Util.TryIntParse(reader["CMSAdminRoleID"]);
			}

			dataHelper.Close(reader);
			dataHelper.Close(command);
			return result;
		}

		public string GetDefaultSearchType()
		{
			return GetAppSettingByName("SearchDefault");
		}

		public void SetDefaultSearchType(string searchType)
		{
			var command = dataHelper.SetCommand("csp_appsetting_update_by_name");
			dataHelper.AddInputParameter("Desc", "SearchDefault", command);
			dataHelper.AddInputParameter("Value", searchType, command);
			dataHelper.ExecuteNonQuery(command);
			dataHelper.Close(command);
		}

		public string Login(string username, string password)
		{
			string message;
			var command = dataHelper.SetCommand("csp_login", false);	// I kinda don't like this "false" optional. This is where my precious snowflake of code is getting all warped. Oh well.
			dataHelper.AddInputParameter("Username", username, command);
			dataHelper.AddInputParameter("Password", password, command);
			dataHelper.AddInputParameter("AdminAppID", 4, command);	// Part of an attempt to consolidate the login of our various apps into one location on NewaysSites

			try
			{
				var result = dataHelper.GetValue(command);
				var resultInt = Util.TryIntParse(result);
				message = resultInt == 1 ? "": "Login Failed. Please check your username and password.";
			}
			catch (Exception ex)
			{
				return "Error:" + ex.Message + "<br/>" + "Inner Exception: " + ex.InnerException;
			}
			finally
			{
				dataHelper.Close(command);
			}

			return message;
		}

		// Admin method (may be upgraded to a core method if the core ever needs an appsetting, but this does not appear to be the case at this time)
		public string GetAppSettingByName(string name)
		{
			var result = string.Empty;
			var command = dataHelper.SetCommand("csp_appsetting_select_by_name");
			dataHelper.AddInputParameter("Desc", name, command);
			var reader = dataHelper.ExecuteReader(command);

			if (reader.Read())
			{
				result = reader["Value"].ToString();
			}

			dataHelper.Close(reader);
			dataHelper.Close(command);
			return result;
		}
	}
}
