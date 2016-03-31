using System;
using System.Collections.Generic;
using System.Data;
using barf.lib.Interface.Data;
using barf.lib.Model;

namespace barf.lib.Data
{
	public class ContentTextDao : IContentTextDao
	{
		private readonly IContentTextRevisionDao revDao;
		private readonly IDataHelper dataHelper;

		public ContentTextDao(IContentTextRevisionDao revDao, IDataHelper dataHelper)
		{
			this.revDao = revDao;
			this.dataHelper = dataHelper;
		}

		// Core method
		public void LoadContentText(ContentText contentText, bool getLatest)
		{
			var command = dataHelper.SetCommand("csp_contenttext_select");
			dataHelper.AddInputParameter("Phrase", contentText.ContentPhrase, command);
			dataHelper.AddInputParameter("LangID", contentText.Language.ID, command);
			dataHelper.AddInputParameter("RoleID", contentText.RoleID, command);
			dataHelper.AddInputParameter("GetLatest", getLatest ? 1 : 0, command);
			var reader = dataHelper.ExecuteReader(command);

			if (reader.Read())
			{
				contentText.ContentType = Util.GetContentType(reader["TypeID"]);
				contentText.StubID = Util.TryIntParse(reader["StubID"]);
				DateTime datetime;
				DateTime.TryParse(reader["GoLiveDateTime"].ToString(), out datetime);
				contentText.GoLiveDate = datetime.ToString("MM/dd/yyyy") == "01/01/0001" ? DateTime.Now.ToString("MM/dd/yyyy") : datetime.ToString("MM/dd/yyyy");

				contentText.GoLiveTime = datetime.ToString("HH:mm");
				contentText.IsLive = datetime <= DateTime.Now;
				contentText.ItemID = Util.TryIntParse(reader["ItemID"]);
				contentText.RoleID = Util.TryIntNullableParse(reader["RoleID"]);
				contentText.ContentTextBlob = reader["Blob"].ToString()
											.Replace("\n", " ")
											.Replace("\v", " ")
											.Replace("\f", " ")
											.Replace("\r", " ");
			}
			else
			{
				InsertContentStubAndText(contentText);
			}

			dataHelper.Close(reader);
			dataHelper.Close(command);
		}

		// Admin method
		public void LoadContentTextMeta(ContentText content)
		{
			var command = dataHelper.SetCommand("csp_contenttext_select_meta");
			dataHelper.AddInputParameter("Phrase", content.ContentPhrase, command);
			dataHelper.AddInputParameter("LangID", content.Language.ID, command);
			dataHelper.AddInputParameter("RoleID", content.RoleID, command);
			var reader = dataHelper.ExecuteReader(command);

			if (reader.Read())
			{
				content.ContentType = Util.GetContentType(reader["TypeID"]);
				content.StubID = Util.TryIntParse(reader["StubID"]);
				content.ItemID = Util.TryIntParse(reader["ItemID"]);
			}

			dataHelper.Close(reader);
			dataHelper.Close(command);
		}

		// Admin method
		public List<ContentText> LoadAll()
		{
			var results = new List<ContentText>();
			var command = dataHelper.SetCommand("csp_contenttext_select_all");
			var reader = dataHelper.ExecuteReader(command);

			while (reader.Read())
			{
				var text = new ContentText();
				SetProperties(text, reader);
				results.Add(text);
			}

			dataHelper.Close(reader);
			dataHelper.Close(command);
			return results;
		}

		/// <summary>
		/// Creates a content stub. If the content's ContentTextBlob is not null, this creates a ContentText as well.
		/// </summary>
		/// <param name="contentText"></param>
		/// <notes>Core method (used inside core method LoadContentText)</notes>
		public void InsertContentStubAndText(ContentText contentText)
		{
			var command = dataHelper.SetCommand("csp_contenttext_insert");
			dataHelper.AddInputParameter("Phrase", contentText.ContentPhrase, command);
			dataHelper.AddInputParameter("StubID", contentText.StubID, command);
			dataHelper.AddInputParameter("ItemID", contentText.ItemID, command);
			dataHelper.AddInputParameter("RoleID", contentText.RoleID, command);
			dataHelper.AddInputParameter("TypeID", (int)contentText.ContentType, command);
			dataHelper.AddInputParameter("Blob", contentText.ContentTextBlob, command);
			dataHelper.AddInputParameter("LangID", contentText.Language.ID, command);
			dataHelper.AddInputParameter("GoLiveDateTime", contentText.GoLiveDateTime, command);
			dataHelper.AddInputParameter("Editor", contentText.Editor, command);
			
			var reader = dataHelper.ExecuteReader(command);

			if (reader.Read())
			{
				contentText.StubID = Util.TryIntParse(reader["StubID"]);
				contentText.ItemID = Util.TryIntParse(reader["ItemID"]);
			}

			dataHelper.Close(reader);
			dataHelper.Close(command);

			if (!string.IsNullOrWhiteSpace(contentText.ContentTextBlob))
			{
				if (contentText.ItemID > 0)
				{
					revDao.Insert(contentText, string.Empty);
				}
				else
				{
					throw new Exception("ItemID not provided");
				}
			}
		}

		// Admin method
		public void LoadTextByID(ContentText text)
		{
			var command = dataHelper.SetCommand("csp_contenttext_select_by_id");
			dataHelper.AddInputParameter("ItemID", text.ItemID, command);
			var reader = dataHelper.ExecuteReader(command);

			if (reader.Read())
			{
				text.ContentPhrase = reader["Phrase"].ToString();
				text.StubID = Util.TryIntParse(reader["StubID"]);
				text.ContentType = Util.GetContentType(reader["TypeID"]);
				var langid = Util.TryIntParse(reader["LangID"]);
				text.Language = new Language(langid);
				text.RoleID = Util.TryIntParse(reader["RoleID"]);
				text.ContentTextBlob = reader["Blob"].ToString();
			}

			dataHelper.Close(reader);
			dataHelper.Close(command);
		}

		// Admin method
		public void UpdateContentStubAndText(ContentText text, string notes)
		{
			var command = dataHelper.SetCommand("csp_contenttext_update");
			dataHelper.AddInputParameter("Phrase", text.ContentPhrase, command);
			dataHelper.AddInputParameter("RoleID", text.RoleID, command);
			dataHelper.AddInputParameter("TypeID", (int)text.ContentType, command);
			dataHelper.AddInputParameter("Blob", text.ContentTextBlob, command);
			dataHelper.AddInputParameter("LanguageID", text.Language.ID, command);
			dataHelper.AddInputParameter("GoLiveDateTime", text.GoLiveDateTime, command);
			dataHelper.AddInputParameter("Editor", text.Editor, command);
			dataHelper.ExecuteNonQuery(command);
			dataHelper.Close(command);
			revDao.Insert(text, notes);
		}

		// Admin method (used for live-edit mode, which I am considering non-core and hence belonging to admin duties)
		public string GetSingleTextContent(string phrase, int? languageID)
		{
			var result = string.Empty;
			var command = dataHelper.SetCommand("csp_contenttext_select_content_only");
			dataHelper.AddInputParameter("LangID", languageID, command);
			dataHelper.AddInputParameter("Phrase", phrase, command);
			var reader = dataHelper.ExecuteReader(command);

			if (reader.Read())
			{
				result = reader["Blob"].ToString();
			}

			dataHelper.Close(reader);
			dataHelper.Close(command);
			return result;
		}

		// Admin method
		public List<ContentText> GetContentItems(int stubID)
		{
			var items = new List<ContentText>();
			var command = dataHelper.SetCommand("csp_contenttext_select_by_stubid");
			dataHelper.AddInputParameter("StubID", stubID, command);
			var reader = dataHelper.ExecuteReader(command);

			while (reader.Read())
			{
				var text = new ContentText();
				text.StubID = stubID;
				text.ItemID = Util.TryIntParse(reader["ItemID"]);
				text.RoleID = Util.TryIntParse(reader["RoleID"]);
				text.ContentTextBlob = reader["Blob"].ToString();
				var langID = Util.TryIntParse(reader["LangID"]);
				text.Language = new Language(langID);
				items.Add(text);
			}

			dataHelper.Close(reader);
			dataHelper.Close(command);
			return items;
		}

		// Helper method
		private static void SetProperties(ContentText content, IDataRecord reader)
		{
			content.StubID = Util.TryIntParse(reader["StubID"]);
			content.ContentPhrase = reader["Phrase"].ToString();
			content.ContentType = Util.GetContentType(reader["TypeID"]);
			content.ItemID = Util.TryIntParse(reader["ItemID"]);
			content.Language.ID = Util.TryIntParse(reader["LangID"]);
			content.RoleID = Util.TryIntParse(reader["RoleID"]);
			content.ContentTextBlob = reader["Blob"].ToString();
		}
	}
}
