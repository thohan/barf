using System.Collections.Generic;
using barf.lib.Model;

namespace barf.lib.Interface.Service
{
	public interface IUtilSvc
	{
		string Login(string username, string password);

		string GetDefaultSearchType();

		void SetDefaultSearchType(string searchType);

		string GetInitialCatalog(string database);

		List<KeyValuePair<string, string>> GetDatabaseSelections();

		List<KeyValuePair<string, string>> GetAdminUserDatabases(string username);

		int GetAdminUserRoleID(string username);
	}
}
