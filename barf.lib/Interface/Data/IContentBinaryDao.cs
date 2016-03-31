using System.Collections.Generic;
using barf.lib.Model;

namespace barf.lib.Interface.Data
{
	public interface IContentBinaryDao
	{
		void LoadContentBinary(ContentBinary binary, bool getLatest);

		void LoadContentBinaryMeta(ContentBinary binary);

		void InsertContentStubAndBinary(ContentBinary binary);

		List<ContentBinary> GetContentItems(int stubID);

		List<ContentBinary> LoadAll();

		void UpdateContentStubAndBinary(ContentBinary binary, string notes);

		int GetSingleBinaryContent(string phrase, int? languageID);

		ContentBinary LoadByContentID(int itemID);
	}
}
