using System.Collections.Generic;
using barf.lib.Model;

namespace barf.lib.Interface.Data
{
	public interface IUtilityDao
	{
		string GetDefaultSearchType();

		void SetDefaultSearchType(string searchType);

		string Login(string username, string password);

		string GetAppSettingByName(string name);

		string GetInitialCatalog(string database);

		List<KeyValuePair<string, string>> GetDatabaseSelections();

		List<KeyValuePair<string, string>> GetAdminUserDatabases(string username);

		int GetAdminUserRoleID(string username);
	}
}
