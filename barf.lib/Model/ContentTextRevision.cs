namespace barf.lib.Model
{
	public class ContentTextRevision
	{
		public ContentTextRevision()
		{

		}

		public ContentTextRevision(int revisionID, int itemID)
		{
			ID = revisionID;
			ContentTextID = itemID;
		}

		public int ID { get; set; }
		public int ContentTextID { get; set; }
		public int LanguageID { get; set; }
		public string DateCreated { get; set; }
		public string Notes { get; set; }
		public string ContentBlob { get; set; }
		public int RoleID { get; set; }

		public string ContentBlobRaw
		{
			get
			{
				return !string.IsNullOrWhiteSpace(ContentBlob) ? ContentBlob.Replace("\n", " ").Replace("\v", " ").Replace("\f", " ").Replace("\r", " ") : string.Empty;
			}
		}

		/// <summary>
		/// This is just a handy way to show an ellipses-shortened piece of text if it happens to be longish.
		/// TODO: This should really be handled in the UI, remove
		/// </summary>
		public string ContentSummary { get; set; }
	}
}
