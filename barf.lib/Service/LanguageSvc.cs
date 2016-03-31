using System;
using System.Collections.Generic;
using barf.lib.Interface.Data;
using barf.lib.Interface.Service;
using barf.lib.Model;

namespace barf.lib.Service
{
	public class LanguageSvc : ILanguageSvc
	{
		private readonly ILanguageDao langDao;

		public LanguageSvc(ILanguageDao langDao)
		{
			this.langDao = langDao;
		}

		public Language GetDefaultLanguage()
		{
			return langDao.GetDefaultLanguage();
		}

		public string SetDefaultLanguage(string languageCode)
		{
			return langDao.SetDefaultLanguage(languageCode);
		}

		public List<Language> GetAllLanguages(int currentLangID)
		{
			return langDao.GetAllLanguages(currentLangID);
		}

		public List<Language> GetActiveLanguages(int currentLangID)
		{
			return langDao.GetActiveLanguages(currentLangID);
		}

		public bool SetActiveLanguage(string languageCode, bool setActive)
		{
			return langDao.SetActiveLanguage(languageCode, setActive);
		}

		public Language GetLanguage(int langID)
		{
			if (langID <= 0)
			{
				throw new Exception("Could not get language, language id was not valid");
			}

			var language = langDao.GetLanguage(langID);

			if (string.IsNullOrWhiteSpace(language.Description))
			{
				language = langDao.GetDefaultLanguage();
			}

			return language;
		}

		public Language GetLanguage(string languageCode)
		{
			if (string.IsNullOrWhiteSpace(languageCode))
			{
				throw new Exception("Could not get language, language code was not provided");
			}

			var language = langDao.GetLanguage(languageCode);

			if (string.IsNullOrWhiteSpace(language.Description))
			{
				language = langDao.GetDefaultLanguage();
			}

			return language;
		}

		/// <summary>
		/// Returns each active language for which there is not yet an existing content piece
		/// </summary>
		/// <param name="stubID"></param>
		/// <param name="currentLangID"></param>
		/// <returns></returns>
		public List<Language> GetUnimplementedLanguages(int stubID, int currentLangID)
		{
			return langDao.GetUnimplementedLanguages(stubID, currentLangID);
		}
	}
}
