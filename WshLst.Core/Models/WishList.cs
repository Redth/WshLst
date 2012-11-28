using System;

namespace WshLst.Core.Models
{
	public class WishList
	{
		public WishList()
		{
			this.Guid = System.Guid.NewGuid().ToString();
			this.Name = null;
			this.Description = string.Empty;
		}

		public int Id { get;set; }
		public string Guid { get; set; }
		public string UserId { get; set; }
		public string Name { get;set; }
		public string Description { get;set; }    
	}
}