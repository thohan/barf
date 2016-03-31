namespace barf.lib.Model
{
	public class ContentText : ContentStub
	{
		public ContentText()
		{

		}

		/// <summary>
		/// Takes an itemID and loads the content accordingly
		/// </summary>
		/// <param name="id"></param>
		public ContentText(int id)
		{
			ItemID = id;
		}

		public int ItemID { get; set; }
		public string ContentTextBlob { get; set; }

		private Language _language;
		public Language Language
		{
			get { return _language ?? (_language = new Language()); }
			set { _language = value; }
		}

		/// <summary>
		/// Raw HTML that is used to load into the CKEditor.
		/// </summary>
		public string ContentTextBlobRaw
		{
			get
			{
				// I just want my html on a single line, that's all
				return !string.IsNullOrWhiteSpace(ContentTextBlob) ? ContentTextBlob.Replace("\n", " ").Replace("\v", " ").Replace("\f", " ").Replace("\r", " ") : string.Empty;
			}
		}

		// TODO: Remove GoLiveDate/Time (or implement it in a way that works)
		public string GoLiveDate { get; set; }
		public string GoLiveTime { get; set; }

		public string GoLiveDateTime
		{
			get { return GoLiveDate + " " + GoLiveTime; }
		}

		public string Editor { get; set; }
	}
}
