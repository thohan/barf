using System;

namespace barf.lib.Model
{
	/// <summary>
	/// This class is pretty much all wrong. See ContentTextRevisionAdapter as a reference.
	/// We aren't storing binary content/files, so this has been neglected.
	/// </summary>
	public class ContentBinaryRevision
	{
		public int ID { get; set; }
		public int ContentBinaryID { get; set; }

		public string FileName { get; set; }
		public string MimeType { get; set; }
		public string Notes { get; set; }
		public DateTime TimeStamp { get; set; }

		public ContentBinaryRevision() { }

		public ContentBinaryRevision(int id)
		{
			ID = id;
		}
	}
}
