using barf.lib.Model;

namespace barf.lib.Interface.Data
{
	public interface IContentBinaryRevisionDao
	{
		void Insert(ContentBinary binary, string notes);

		void Load(ContentBinaryRevision revision);
	}
}
