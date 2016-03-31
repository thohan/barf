using System;
using System.Collections.Generic;
using System.Threading;
using barf.lib.Interface.Data;
using barf.lib.Interface.Service;
using barf.lib.Model;

namespace barf.lib.Service
{
	public class ContentSvc : IContentSvc
	{
		private readonly IContentStubDao stubDao;
		private readonly IContentBinaryDao binaryDao;
		private readonly IContentTextDao textDao;
		private readonly ISessionSvc sessionSvc;
		private readonly ILanguageSvc languageSvc;

		public ContentSvc(IContentStubDao stubDao, IContentBinaryDao binaryDao, IContentTextDao textDao, ISessionSvc sessionSvc, ILanguageSvc languageSvc)
		{
			this.stubDao = stubDao;
			this.binaryDao = binaryDao;
			this.textDao = textDao;
			this.sessionSvc = sessionSvc;
			this.languageSvc = languageSvc;
		}

		/// <summary>
		/// This'll be used on the content list page
		/// </summary>
		/// <param name="filterValue"></param>
		/// <param name="searchType"></param>
		/// <param name="adminRoleID"></param>
		/// <returns></returns>
		public ContentStubList GetContentStubs(string filterValue, string searchType, int adminRoleID)
		{
			return stubDao.GetContentStubs(filterValue, searchType, adminRoleID);
		}

		/// <summary>
		/// This method is only for saving a new content piece, not for updating (Admin method)
		/// </summary>
		public bool SaveNewStub(ContentStub stub)
		{
			var result = false;

			if (!string.IsNullOrWhiteSpace(stub.ContentPhrase))
			{
				stub.StubID = stubDao.GetStubID(stub.ContentPhrase);

				if (stub.StubID == 0)
				{
					if (stub.ContentType != ContentType.unknown)
					{
						stubDao.InsertContent(stub);
						result = true;
					}
				}
			}

			return result;
		}

		public bool UpdateStub(ContentStub stub)
		{
			var result = false;

			if (!string.IsNullOrWhiteSpace(stub.ContentPhrase))
			{
				if (stub.StubID > 0 && stub.ContentType != ContentType.unknown)
				{
					stubDao.UpdateContent(stub);
					result = true;
				}
			}

			return result;
		}

		/// <summary>
		/// This deletes content stub, all associated content pieces and all associated content revisions.
		/// Are you sure you want to do this?
		/// </summary>
		/// <returns></returns>
		public bool Remove(string phrase)
		{
			var result = false;

			if (!string.IsNullOrWhiteSpace(phrase))
			{
				stubDao.DeleteContent(phrase);
				result = true;
			}

			return result;
		}

		/// <summary>
		/// This whole thing concerns cache and really doesn't belong necessarily at this service level
		/// </summary>
		/// <param name="stub"></param>
		/// <param name="langID"></param>
		/// <returns></returns>
		public ContentStub GetItem(ContentStub stub, int langID = 0)
		{
			//// Disabling cache for now. I need to ensure that cache is updated when I edit content!
			//if (cacheSvc.ContentTextCache != null && cacheSvc.ContentBinaryCache != null && false)
			//{
			//	var item = GetFromCache(stub, languageID);

			//	// Now I have to check to see if GetFromCache returns null. If so, get from database and add to cache.
			//	if (item == null)
			//	{
			//		item = GetFromDatabase(stub, languageID);

			//		switch (item.ContentType)
			//		{
			//			case ContentType.plain:
			//			case ContentType.rich:
			//				cacheSvc.ContentTextCache.Add((ContentText)item);
			//				break;
			//			case ContentType.image:
			//				cacheSvc.ContentBinaryCache.Add((ContentBinary)item);
			//				break;
			//		}
			//	}

			//	return item;
			//}

			return GetFromDatabase(stub, false, langID);
		}

		public ContentStub GetLatestItem(ContentStub stub, int langID = 0)
		{
			return GetFromDatabase(stub, true, langID);
		}

		private ContentStub GetFromDatabase(ContentStub stub, bool getLatest, int langID)
		{
			// Would it make sense to consolidate the procs, or just have switch statements here and there like this one?
			// I'd really rather centralize such things. However, a quick search shows that there are only two switch statements in the project.
			var currentLanguage = languageSvc.GetLanguage(langID);

			switch (stub.ContentType)
			{
				case ContentType.text:
					var text = new ContentText();
					text.ContentType = stub.ContentType;
					text.ContentPhrase = stub.ContentPhrase;
					text.Language = currentLanguage;
					text.Editor = sessionSvc.CurrentUser;
					textDao.LoadContentText(text, getLatest);
					return text;
				case ContentType.binary:
					var binary = new ContentBinary();
					binary.ContentType = stub.ContentType;
					binary.ContentPhrase = stub.ContentPhrase;
					binary.Language = currentLanguage;
					binary.Editor = sessionSvc.CurrentUser;
					binaryDao.LoadContentBinary(binary, getLatest);
					return binary;
				default:
					return null;
			}
		}

		public string SaveBinary(ContentBinary binary, string notes)
		{
			string result;

			DateTime dateTime;
			DateTime.TryParse(binary.GoLiveDateTime, out dateTime);

			if (dateTime == DateTime.MinValue)
			{
				return "Go-live date and/or time was invalid";
			}

			if (!string.IsNullOrWhiteSpace(binary.ContentPhrase) && binary.IsBinary)
			{
				result = "";
				binaryDao.LoadContentBinaryMeta(binary);

				binary.Editor = sessionSvc.CurrentUser;

				if (binary.ItemID == 0)
				{
					binaryDao.InsertContentStubAndBinary(binary);
				}
				else
				{
					binaryDao.UpdateContentStubAndBinary(binary, notes);
				}
			}
			else
			{
				result = "Content phrase was empty or content type was incorrect";
			}

			return result;
		}

		public ContentBinary LoadBinaryByContentID(int itemID)
		{
			var binary = binaryDao.LoadByContentID(itemID);

			// The proc to get language information was causing some issues, so I'm doing this language finagling instead
			var langID = binary.Language.ID;
			binary.Language = sessionSvc.ActiveLanguages.Find(x => x.ID == langID);
			
			if (binary.Language == null || string.IsNullOrWhiteSpace(binary.Language.LanguageCode))
			{
				binary.Language = languageSvc.GetLanguage(langID);
			}

			return binary;
		}

		public string SaveText(ContentText text, string notes)
		{
			string result;
			DateTime dateTime;
			DateTime.TryParse(text.GoLiveDateTime, out dateTime);

			if (dateTime == DateTime.MinValue)
			{
				//// This is preventing content from getting saved. People aren't really using the GoLive date time thing at this point
				// TODO: Get GoLive dates working, or remove the code referencing it.
				//return "Go-live date and/or time was invalid";
			}

			text.Editor = sessionSvc.CurrentUser;

			if (!string.IsNullOrWhiteSpace(text.ContentPhrase) && text.IsText)
			{
				result = "";
				// This method doesn't modify the contentBlob (I may not want to modify language either, we'll see...)
				textDao.LoadContentTextMeta(text);

				// This doesn't really make sense anymore. The stub and ContentText can exist, but the ContentTextRole (actual content) may not yet exist.
				// I guess that's the criteria I'll continue to work with until such time as it no longer makes sense.
				if (text.ItemID == 0)
				{
					// If you end up here, it means the contentphrase wasn't found in the db. This path should never be reached.
					textDao.InsertContentStubAndText(text);
				}
				else
				{
					textDao.UpdateContentStubAndText(text, notes);
				}
			}
			else
			{
				result = "Content phrase was empty or content type was incorrect";
			}

			return result;
		}

		/// <summary>
		/// Returns all items associated with the given stub ID
		/// </summary>
		/// <param name="stubID"></param>
		/// <returns></returns>
		public List<ContentText> GetTextItems(int stubID)
		{
			var contentItems = textDao.GetContentItems(stubID);

			foreach (var contentItem in contentItems)
			{
				var langID = contentItem.Language.ID;
				contentItem.Language = sessionSvc.ActiveLanguages.Find(x => x.ID == langID);

				if (contentItem.Language == null || string.IsNullOrWhiteSpace(contentItem.Language.LanguageCode))
				{
					contentItem.Language = languageSvc.GetLanguage(langID);
				}
			}

			return contentItems;
		}

		public List<ContentBinary> GetBinaryItems(int stubID)
		{
			var binaries = binaryDao.GetContentItems(stubID);

			foreach (var binary in binaries)
			{
				var langID = binary.Language.ID;
				binary.Language = sessionSvc.ActiveLanguages.Find(x => x.ID == langID);

				if (binary.Language == null || string.IsNullOrWhiteSpace(binary.Language.LanguageCode))
				{
					binary.Language = languageSvc.GetLanguage(langID);
				}
			}

			return binaries;
		}

		public bool LoadText(ContentText text)
		{
			if (!string.IsNullOrWhiteSpace(text.ContentPhrase))
			{
				text.Editor = sessionSvc.CurrentUser;
				textDao.LoadContentText(text, false);
				return true;
			}

			return false;
		}

		public void LoadTextByID(ContentText text)
		{
			textDao.LoadTextByID(text);
			var langID = text.Language.ID;
			text.Language = sessionSvc.ActiveLanguages.Find(x => x.ID == langID);

			if (text.Language == null || string.IsNullOrWhiteSpace(text.Language.LanguageCode))
			{
				text.Language = languageSvc.GetLanguage(langID);
			}
		}

		/// <summary>
		/// This method is to get a single text content piece that has been updated. Only applies to edit mode and the contentList page
		/// </summary>
		public string GetSingleTextContent(string contentPhrase, int? languageID)
		{
			var currentLanguage = languageSvc.GetLanguage(Thread.CurrentThread.CurrentUICulture.ToString());
			var langid = languageID ?? currentLanguage.ID;
			return textDao.GetSingleTextContent(contentPhrase, langid);
		}

		public int GetSingleBinaryContent(string contentPhrase, int? languageID)
		{
			var currentLanguage = languageSvc.GetLanguage(Thread.CurrentThread.CurrentUICulture.ToString());
			var langid = languageID ?? currentLanguage.ID;
			return binaryDao.GetSingleBinaryContent(contentPhrase, langid);
		}

		public List<ContentText> GetAllTextItems()
		{
			return textDao.LoadAll();
		}
	}
}
