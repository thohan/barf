using System.Collections.Generic;
using barf.lib.Model;

namespace barf.lib.Interface.Service
{
	public interface ILanguageSvc
	{
		Language GetDefaultLanguage();

		string SetDefaultLanguage(string languageCode);

		List<Language> GetAllLanguages(int currentLangID);

		List<Language> GetActiveLanguages(int currentLangID);

		bool SetActiveLanguage(string languageCode, bool setActive);

		Language GetLanguage(int langID);

		Language GetLanguage(string languageCode);

		List<Language> GetUnimplementedLanguages(int stubID, int currentLangID);
	}
}
