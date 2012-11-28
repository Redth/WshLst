using System;

namespace WshLst.Core.Models
{
	public class EntryImage
	{
		public EntryImage()
		{
			ImageGuid = Guid.NewGuid().ToString();
			ImageBase64 = string.Empty;
		}

		public int Id { get; set; }
		public string UserId { get; set; }
		public string ImageGuid { get; set; }
		public string ImageBase64 { get; set; }
	}
}