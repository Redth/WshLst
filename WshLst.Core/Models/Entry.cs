namespace WshLst.Core.Models
{
	public class Entry
	{
		public Entry()
		{
			Store = string.Empty;
			Upc = string.Empty;
			Notes = string.Empty;
			UserId = string.Empty;
			ImageGuid = string.Empty;
		}

		public int Id { get; set; }
		public string UserId { get; set; }
		public int ListId { get; set; }
		public string Name { get; set; }
		public double? Longitude { get; set; }
		public double? Latitude { get; set; }
		public string Store { get; set; }
		public string Upc { get; set; }
		public double? Price { get; set; }
		public string Notes { get; set; }

		public string ImageGuid { get; set; }

		public bool HasImage
		{
			get { return !string.IsNullOrEmpty(ImageGuid); }
		}
	}
}