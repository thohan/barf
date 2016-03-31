using System.Collections.Generic;
using barf.lib.Model;

namespace barf.lib.Interface.Service
{
	public interface ISessionSvc
	{
		string InitialCatalog { get; set; }

		Language CurrentLanguage { get; set; }

		List<Language> ActiveLanguages { get; set; }

		string CurrentUser { get; set; }

		int CurrentAdminUserRoleID { get; set; }

		List<KeyValuePair<string, string>> CurrentDatabases { get; set; }

		bool IsLoggedOn { get; set; }

		bool IsEditMode { get; set; }
	}
}
