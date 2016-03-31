using barf.lib.Interface.Data;
using barf.lib.Model;

namespace barf.lib.Data
{
	public class ContentStubDao : IContentStubDao
	{
		private readonly IDataHelper dataHelper;

		public ContentStubDao(IDataHelper dataHelper)
		{
			this.dataHelper = dataHelper;
		}

		// Admin method
		public int GetStubID(string contentPhrase)
		{
			var result = 0;
			var command = dataHelper.SetCommand("csp_contentstub_select_id");
			dataHelper.AddInputParameter("Phrase", contentPhrase, command);
			var reader = dataHelper.ExecuteReader(command);

			if (reader.Read())
			{
				result = (int)reader["StubID"];
			}

			dataHelper.Close(reader);
			dataHelper.Close(command);
			return result;
		}

		/// <summary>
		/// This method is used just to save a content stub, and the only thing known are content type and content phrase
		/// </summary>
		/// <param name="content"></param>
		/// <notes>Admin method</notes>
		public void InsertContent(ContentStub content)
		{
			var command = dataHelper.SetCommand("csp_contentstub_insert");
			dataHelper.AddInputParameter("Phrase", content.ContentPhrase, command);
			dataHelper.AddInputParameter("TypeID", content.ContentType, command);
			content.StubID = (int)dataHelper.GetScalarValue(command);
			dataHelper.Close(command);
		}

		// Admin method
		public void UpdateContent(ContentStub contentStub)
		{
			var command = dataHelper.SetCommand("csp_contentstub_update");
			dataHelper.AddInputParameter("ID", contentStub.StubID, command);
			dataHelper.AddInputParameter("Phrase", contentStub.ContentPhrase, command);
			dataHelper.AddInputParameter("TypeID", contentStub.ContentType, command);
			dataHelper.ExecuteNonQuery(command);
			dataHelper.Close(command);
		}

		// Admin method
		public void DeleteContent(string phrase)
		{
			var command = dataHelper.SetCommand("csp_contentstub_delete");
			dataHelper.AddInputParameter("Phrase", phrase, command);
			dataHelper.ExecuteNonQuery(command);
			dataHelper.Close(command);
		}

		// Admin method
		public ContentStubList GetContentStubs(string filterValue, string searchType, int adminRoleID)
		{
			var results = new ContentStubList();
			var command = dataHelper.SetCommand("csp_contentstub_select_all");
			dataHelper.AddInputParameter("FilterValue", filterValue, command);
			dataHelper.AddInputParameter("SearchType", searchType, command);
			dataHelper.AddInputParameter("AdminRoleID", adminRoleID, command);
			var reader = dataHelper.ExecuteReader(command);

			while (reader.Read())
			{
				var stub = new ContentStub();
				stub.StubID = Util.TryIntParse(reader["ID"]);
				stub.ContentPhrase = reader["Phrase"].ToString();
				stub.ContentType = Util.GetContentType(reader["TypeID"]);
				results.Add(stub);
			}

			dataHelper.Close(reader);
			dataHelper.Close(command);
			return results;
		}
	}
}
