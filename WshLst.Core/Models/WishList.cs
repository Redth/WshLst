namespace WshLst.Core.Models
{
	public class WishList
	{
		public WishList()
		{
			Guid = System.Guid.NewGuid().ToString();
			Name = null;
			Description = string.Empty;
		}

		public int Id { get; set; }
		public string Guid { get; set; }
		public string UserId { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
	}
}