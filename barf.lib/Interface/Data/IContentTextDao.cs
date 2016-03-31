using System.Collections.Generic;
using barf.lib.Model;

namespace barf.lib.Interface.Data
{
	public interface IContentTextDao
	{
		void LoadContentText(ContentText text, bool getLatest);

		void LoadContentTextMeta(ContentText text);

		List<ContentText> LoadAll();

		void InsertContentStubAndText(ContentText text);

		void LoadTextByID(ContentText text);

		void UpdateContentStubAndText(ContentText text, string notes);

		string GetSingleTextContent(string phrase, int? languageID);

		List<ContentText> GetContentItems(int stubID);
	}
}
