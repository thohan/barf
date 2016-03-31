using System;

namespace barf.lib.Model
{
	public static class Util
	{
		public static int TryIntParse(object intstring)
		{
			int retval;
			int.TryParse(intstring.ToString(), out retval);
			return retval;
		}

		public static int? TryIntNullableParse(object intstring)
		{
			if (string.IsNullOrWhiteSpace(intstring.ToString()))
			{
				return null;
			}

			int retval;
			int.TryParse(intstring.ToString(), out retval);
			return retval;
		}

		public static ContentType GetContentType(object contentType)
		{
			ContentType ctype;
			Enum.TryParse(contentType.ToString(), out ctype);
			return ctype;
		}
	}
}
