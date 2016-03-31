using System.Web.Mvc;

namespace barf.lib.Model
{
	public class ImageResult : ActionResult
	{
		public string ContentType { get; set; }
		public byte[] ImageBytes { get; set; }
		public string SourceFilename { get; set; }

		//This is used for times where you have a physical location
		public ImageResult(string sourceFilename, string contentType)
		{
			SourceFilename = sourceFilename;
			ContentType = contentType;
		}

		//This is used for when you have the image in byte form
		public ImageResult(byte[] sourceStream, string contentType)
		{
			ImageBytes = sourceStream;
			ContentType = contentType;
		}

		public override void ExecuteResult(ControllerContext context)
		{
			var response = context.RequestContext.HttpContext.Response;

			response.Clear();
			//response.Cache.SetCacheability(HttpCacheability.NoCache);
			response.ContentType = ContentType;

			if (ImageBytes != null)
			{
				response.OutputStream.Write(ImageBytes, 0, ImageBytes.Length);
			}

			response.End();
			//Check to see if this is done from bytes or physical location
			//  If you're really paranoid you could set a true/false flag in
			//  the constructor.
			//if (ImageBytes != null)
			//{
			//    var stream = new MemoryStream(ImageBytes);
			//    stream.WriteTo(response.OutputStream);
			//    stream.Dispose();
			//}
			//else
			//{
			//    response.TransmitFile(SourceFilename);
			//}
		}
	}
}
