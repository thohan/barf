using barf.lib.Model;

namespace barf.lib.Interface.Data
{
	public interface IContentStubDao
	{
		int GetStubID(string contentPhrase);

		void InsertContent(ContentStub stub);

		void UpdateContent(ContentStub stub);

		void DeleteContent(string phrase);

		ContentStubList GetContentStubs(string filterValue, string searchType, int adminRoleID);
	}
}
