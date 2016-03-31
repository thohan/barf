using System;
using System.Collections.Generic;
using barf.lib.Interface.Data;
using barf.lib.Interface.Service;

namespace barf.lib.Service
{
	public class UtilSvc : IUtilSvc
	{
		private readonly IUtilityDao utilityDao;

		public UtilSvc(IUtilityDao utilityDao)
		{
			this.utilityDao = utilityDao;
		}

		public string Login(string username, string password)
		{
			return utilityDao.Login(username, password);
		}

		public string GetDefaultSearchType()
		{
			return utilityDao.GetDefaultSearchType();
		}

		public void SetDefaultSearchType(string searchType)
		{
			utilityDao.SetDefaultSearchType(searchType);
		}

		public string GetInitialCatalog(string database)
		{
			return utilityDao.GetInitialCatalog(database);
		}

		public List<KeyValuePair<string, string>> GetDatabaseSelections()
		{
			return utilityDao.GetDatabaseSelections();
		}

		public List<KeyValuePair<string, string>> GetAdminUserDatabases(string username)
		{
			return utilityDao.GetAdminUserDatabases(username);
		}

		public int GetAdminUserRoleID(string username)
		{
			return utilityDao.GetAdminUserRoleID(username);
		}

		public static int TryIntParse(object intstring)
		{
			int retval;
			int.TryParse(intstring.ToString(), out retval);
			return retval;
		}

		public static int? TryIntNullableParse(object intstring)
		{
			if (string.IsNullOrWhiteSpace(intstring.ToString()))
			{
				return null;
			}

			int retval;
			int.TryParse(intstring.ToString(), out retval);
			return retval;
		}

		public static ContentType GetContentType(object contentType)
		{
			ContentType ctype;
			Enum.TryParse(contentType.ToString(), out ctype);
			return ctype;
		}
	}
}
