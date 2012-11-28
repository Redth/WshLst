using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WshLst.Core.Models;

namespace WshLst.Web.ViewModels
{
	public class WishListViewModel
	{
		HttpClient http;
    string azureMobileServiceUrl = "";
    
		public WishListViewModel(string wishListGuid)
		{
			this.WishListGuid = wishListGuid;

			this.WishList = new WishList();
			this.Entries = new List<Entry>();

			this.EntryImages = new Dictionary<string, EntryImage>();

      //TODO: Load key and url from config.json
      var azureMobileServiceAppKey = "";
      azureMobileServiceUrl = "";
      
			http = new HttpClient();
			http.DefaultRequestHeaders.Add("X-ZUMO-APPLICATION", azureMobileServiceAppKey);
			http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

		}

		public string WishListGuid { get; set; }

		public WishList WishList { get; set; }
		public IEnumerable<Entry> Entries { get; set; }
		public Dictionary<string, EntryImage> EntryImages { get; set; }

		public async Task LoadList()
		{
			var url = azureMobileServiceUrl + "/tables/WishList?$filter=Guid eq '" + this.WishListGuid + "'";
			
			var data = await http.GetStringAsync(url);

			this.WishList = JsonConvert.DeserializeObject<IEnumerable<WishList>>(data).FirstOrDefault();
		}

		public async Task LoadEntries()
		{
			if (WishList == null)
				return;

			var url = azureMobileServiceUrl + "/tables/Entry?$filter=ListId eq '" + this.WishList.Id + "'";

			var data = await http.GetStringAsync(url);

			this.Entries = JsonConvert.DeserializeObject<IEnumerable<Entry>>(data);

			EntryImages.Clear();

			//Get entry images in batches of 5 images per azure rest api call
			int pos = 0;
			while (true)
			{
				var entriesOn = Entries.Where(e => !string.IsNullOrEmpty(e.ImageGuid)).Skip(pos++).Take(5);

				if (entriesOn == null || entriesOn.Count() <= 0)
					break;

				var filter = string.Join(" or ",
					from e in entriesOn
					select "ImageGuid eq '" + e.ImageGuid + "'");

				url = azureMobileServiceUrl + "/tables/EntryImage?$filter=" + filter;

				data = await http.GetStringAsync(url);

				var entryImgs = JsonConvert.DeserializeObject<List<EntryImage>>(data);

				foreach (var ei in entryImgs)
					EntryImages.Add(ei.ImageGuid, ei);

				pos += 5;
			}			
		}
	}
}