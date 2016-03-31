using System.Collections.Generic;
using barf.lib.Model;

namespace barf.lib.Interface.Data
{
	public interface ILanguageDao
	{
		Language GetDefaultLanguage();

		Language GetLanguage(int languageID);

		Language GetLanguage(string languageCode);

		List<Language> GetAllLanguages(int currentLangID);

		List<Language> GetActiveLanguages(int currentLangID);

		List<Language> GetUnimplementedLanguages(int stubID, int currentLangID);

		bool SetActiveLanguage(string languageCode, bool setActive);

		string SetDefaultLanguage(string languageCode);
	}
}
