using System.Collections.Generic;
using barf.lib.Model;

namespace barf.lib.Interface.Service
{
	public interface IContentSvc
	{
		ContentStubList GetContentStubs(string filterValue, string searchType, int adminRoleID);

		bool SaveNewStub(ContentStub stub);

		bool UpdateStub(ContentStub stub);

		bool Remove(string phrase);

		ContentStub GetItem(ContentStub stub, int languageID);

		ContentStub GetLatestItem(ContentStub stub, int langID = 0);

		string SaveBinary(ContentBinary binary, string notes);

		List<ContentBinary> GetBinaryItems(int stubID);

		ContentBinary LoadBinaryByContentID(int itemID);

		string SaveText(ContentText text, string notes);

		List<ContentText> GetTextItems(int stubID);

		bool LoadText(ContentText text);

		void LoadTextByID(ContentText text);

		string GetSingleTextContent(string phrase, int? langID);

		int GetSingleBinaryContent(string phrase, int? langID);

		List<ContentText> GetAllTextItems();
	}
}
