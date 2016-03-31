using System.Collections.Generic;
using System.Threading;
using System.Web;
using barf.lib.Interface.Data;
using barf.lib.Interface.Service;
using barf.lib.Model;

namespace barf.lib.Service
{
	// TODO: We should be able to do what we need without Session, consider replacing this whole class.
	public class SessionSvc : ISessionSvc
	{
		private readonly ILanguageDao langDao;

		public SessionSvc(ILanguageDao langDao)
		{
			this.langDao = langDao;
		}

		public string InitialCatalog
		{
			get
			{
				if (HttpContext.Current == null || HttpContext.Current.Session == null)
				{
					return null;
				}

				if (HttpContext.Current.Session["InitialCatalog"] == null)
				{
					HttpContext.Current.Session["InitialCatalog"] = string.Empty;
				}

				return HttpContext.Current.Session["InitialCatalog"].ToString();
			}
			set
			{
				if (HttpContext.Current == null || HttpContext.Current.Session == null)
				{
					return;
				}

				HttpContext.Current.Session["InitialCatalog"] = value;
			}
		}

		public bool IsEditMode
		{
			get
			{
				if (HttpContext.Current.Session["DCMSIsEditMode"] != null)
				{
					bool tryBool;
					bool.TryParse(HttpContext.Current.Session["DCMSIsEditMode"].ToString(), out tryBool);
					return tryBool;
				}

				return false;
			}
			set
			{
				HttpContext.Current.Session["DCMSIsEditMode"] = value;
			}
		}

		public Language CurrentLanguage
		{
			get
			{
				if (HttpContext.Current == null || HttpContext.Current.Session == null)
				{
					return null;
				}

				if (HttpContext.Current.Session["DCMSLanguage"] == null)
				{
					HttpContext.Current.Session["DCMSLanguage"] = langDao.GetDefaultLanguage();
				}

				return (Language)HttpContext.Current.Session["DCMSLanguage"];
			}
			set
			{
				if (HttpContext.Current == null || HttpContext.Current.Session == null)
				{
					return;
				}

				HttpContext.Current.Session["DCMSLanguage"] = value;
			}
		}

		public List<Language> ActiveLanguages
		{
			get
			{
				if (HttpContext.Current == null || HttpContext.Current.Session == null)
				{
					return null;
				}

				if (HttpContext.Current.Session["DCMSActiveLanguages"] == null)
				{
					var currentLanguage = langDao.GetLanguage(Thread.CurrentThread.CurrentUICulture.ToString());
					HttpContext.Current.Session["DCMSActiveLanguages"] = langDao.GetActiveLanguages(currentLanguage.ID);	// Why would I need current ID to get active languages? To set the current one in the list.
				}

				return (List<Language>)HttpContext.Current.Session["DCMSActiveLanguages"];
			}
			set
			{
				if (HttpContext.Current == null || HttpContext.Current.Session == null)
				{
					return;
				}

				HttpContext.Current.Session["DCMSActiveLanguages"] = value;
			}
		}

		public string CurrentUser
		{
			get
			{
				if (HttpContext.Current == null || HttpContext.Current.Session == null)
				{
					return null;
				}

				if (HttpContext.Current.Session["DCMSUser"] == null)
				{
					HttpContext.Current.Session["DCMSUser"] = string.Empty;
				}

				return HttpContext.Current.Session["DCMSUser"].ToString();
			}
			set
			{
				if (HttpContext.Current == null || HttpContext.Current.Session == null)
				{
					return;
				}

				HttpContext.Current.Session["DCMSUser"] = value;
			}
		}

		public int CurrentAdminUserRoleID
		{
			get
			{
				if (HttpContext.Current == null || HttpContext.Current.Session == null)
				{
					return 0;
				}

				if (HttpContext.Current.Session["DCMSUserRoleID"] == null)
				{
					HttpContext.Current.Session["DCMSUserRoleID"] = 0;
				}

				return (int)HttpContext.Current.Session["DCMSUserRoleID"];
			}
			set
			{
				if (HttpContext.Current == null || HttpContext.Current.Session == null)
				{
					return;
				}

				HttpContext.Current.Session["DCMSUserRoleID"] = value;
			}
		}

		public List<KeyValuePair<string, string>> CurrentDatabases
		{
			get
			{
				if (HttpContext.Current == null || HttpContext.Current.Session == null)
				{
					return null;
				}

				if (HttpContext.Current.Session["DCMSUserDatabases"] == null)
				{
					HttpContext.Current.Session["DCMSUserDatabases"] = null;
				}

				return (List<KeyValuePair<string, string>>)HttpContext.Current.Session["DCMSUserDatabases"];
			}
			set
			{
				if (HttpContext.Current == null || HttpContext.Current.Session == null)
				{
					return;
				}

				HttpContext.Current.Session["DCMSUserDatabases"] = value;
			}
		}

		// This'll be replaced by something better
		public bool IsLoggedOn
		{
			get
			{
				if (HttpContext.Current == null || HttpContext.Current.Session == null)
					return false;

				if (HttpContext.Current.Session["DCMSIsLoggedOn"] == null)
					return false;

				return (bool)HttpContext.Current.Session["DCMSIsLoggedOn"];
			}
			set
			{
				HttpContext.Current.Session["DCMSIsLoggedOn"] = value;
			}
		}
	}
}
