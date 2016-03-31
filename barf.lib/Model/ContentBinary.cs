namespace barf.lib.Model
{
	public class ContentBinary : ContentStub
	{
		public int ItemID { get; set; }

		public int ContentID { get; set; }

		public byte[] ContentBinaryBlob { get; set; }

		private Language _language;
		public Language Language
		{
			get { return _language ?? (_language = new Language()); }
			set { _language = value; }
		}

		public string MimeType { get; set; }
		public string FileName { get; set; }

		public byte[] Thumbnail { get; set; }

		public ContentBinary() { }

		public ContentBinary(int id)
		{
			ContentID = id;
		}

		// TODO: Remove GoLiveDateTime from the code/schema
		public string GoLiveDate { get; set; }
		public string GoLiveTime { get; set; }
		public string GoLiveDateTime
		{
			get { return GoLiveDate + " " + GoLiveTime; }
		}

		public string Editor { get; set; }
	}
}
