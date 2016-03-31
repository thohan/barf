using System;
using System.Collections.Generic;
using System.Data;
using barf.lib.Interface.Data;
using barf.lib.Model;

namespace barf.lib.Data
{
	public class LanguageDao : ILanguageDao
	{
		private readonly IDataHelper dataHelper;

		public LanguageDao(IDataHelper dataHelper)
		{
			this.dataHelper = dataHelper;
		}

		// Core method (the extensions are riddled with this guy)
		public Language GetDefaultLanguage()
		{
			var language = new Language();
			var command = dataHelper.SetCommand("csp_languages_select_default");
			var reader = dataHelper.ExecuteReader(command);

			if (reader.Read())
			{
				SetProperties(language, reader);
			}

			dataHelper.Close(reader);
			dataHelper.Close(command);
			return language;
		}

		// The website will presumably have it's own list of valid culture codes representing languages we can support in the CMS.
		// I don't need to store them in the cms database. But, I seem to be querying passing in a default language of 0 in some cases.
		// Until that's refactored, I'll need this method intact.
		public Language GetLanguage(int langID)
		{
			var language = new Language();
			var command = dataHelper.SetCommand("csp_languages_select_by_id");
			dataHelper.AddInputParameter("LangID", langID, command);
			var reader = dataHelper.ExecuteReader(command);

			if (reader.Read())
			{
				SetProperties(language, reader);
			}

			dataHelper.Close(reader);
			dataHelper.Close(command);
			return language;
		}

		// This is core, used by the razor extensions
		public Language GetLanguage(string langCode)
		{
			var language = new Language();
			var command = dataHelper.SetCommand("csp_languages_select_by_code");
			dataHelper.AddInputParameter("LangCode", langCode, command);
			var reader = dataHelper.ExecuteReader(command);

			if (reader.Read())
			{
				SetProperties(language, reader);
			}

			dataHelper.Close(reader);
			dataHelper.Close(command);
			return language;
		}

		// Admin method
		public List<Language> GetAllLanguages(int currentLangID)
		{
			var languages = new List<Language>();
			var command = dataHelper.SetCommand("csp_languages_select_all");
			var reader = dataHelper.ExecuteReader(command);

			while (reader.Read())
			{

				SetLanguageListProperties(languages, reader, currentLangID);
			}

			dataHelper.Close(reader);
			dataHelper.Close(command);
			return languages;
		}

		// Admin method (just returns all the currently-active languages for the language admin page)
		public List<Language> GetActiveLanguages(int currentLangID)
		{
			var languages = new List<Language>();
			var command = dataHelper.SetCommand("csp_languages_select_active");
			var reader = dataHelper.ExecuteReader(command);

			while (reader.Read())
			{
				SetLanguageListProperties(languages, reader, currentLangID);
			}

			dataHelper.Close(reader);
			dataHelper.Close(command);
			return languages;
		}

		// Admin
		public List<Language> GetUnimplementedLanguages(int stubID, int currentLangID)
		{
			var langs = new List<Language>();
			var command = dataHelper.SetCommand("csp_languages_get_unused_for_stub");
			dataHelper.AddInputParameter("StubID", stubID, command);
			var reader = dataHelper.ExecuteReader(command);

			while (reader.Read())
			{
				var lang = new Language();
				lang.ID = Util.TryIntParse(reader["LangID"]);
				lang.LanguageCode = reader["Code"].ToString();
				lang.Description = reader["Desc"].ToString();
				lang.IsActive = true;
				lang.IsCurrent = lang.ID == currentLangID;
				langs.Add(lang);
			}

			dataHelper.Close(reader);
			dataHelper.Close(command);
			return langs;
		}

		// Admin method (it adds a language to the list of active languages)
		public bool SetActiveLanguage(string languageCode, bool setActive)
		{
			int rowcount;
			var command = dataHelper.SetCommand("csp_languages_set_active");
			dataHelper.AddInputParameter("LangCode", languageCode, command);
			dataHelper.AddInputParameter("IsActive", setActive, command);
			dataHelper.AddOutputParameter("RowCount", command);

			try
			{
				dataHelper.ExecuteNonQuery(command);
				rowcount = dataHelper.GetInt32ReturnValue("RowCount", command);
			}
			catch
			{
				dataHelper.Close(command);
				return false;
			}

			var result = rowcount == 1;
			dataHelper.Close(command);
			return result;
		}

		// Admin (this sets default via admin [as opposed to the CurrentLanguage which a user can set])
		public string SetDefaultLanguage(string languageCode)
		{
			var result = string.Empty;
			var command = dataHelper.SetCommand("csp_appsetting_update_by_name");
			dataHelper.AddInputParameter("Desc", "DefaultLanguage", command);
			dataHelper.AddInputParameter("Value", languageCode, command);

			try
			{
				dataHelper.ExecuteNonQuery(command);
			}
			catch (Exception ex)
			{
				dataHelper.Close(command);
				result = ex.Message;
				return result;
			}

			dataHelper.Close(command);
			SetActiveLanguage(languageCode, true);
			return result;
		}

		private static void SetProperties(Language language, IDataRecord reader)
		{
			language.ID = Util.TryIntParse(reader["LangID"]);
			language.LanguageCode = reader["Code"].ToString();
			language.Description = reader["Desc"].ToString();
			var isactive = Util.TryIntParse(reader["IsActive"]);
			language.IsActive = isactive == 1;
		}

		private void SetLanguageListProperties(ICollection<Language> languages, IDataRecord reader, int currentLangID)
		{
			var lang = new Language();
			lang.ID = Util.TryIntParse(reader["LangID"]);
			lang.LanguageCode = reader["Code"].ToString();
			lang.Description = reader["Desc"].ToString();
			var isActive = Util.TryIntParse(reader["IsActive"]);
			lang.IsActive = isActive > 0;
			lang.IsCurrent = lang.ID == currentLangID;
			languages.Add(lang);
		}
	}
}
