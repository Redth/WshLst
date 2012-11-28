using System;
using System.Net;
using System.Threading.Tasks;
using Cirrious.MvvmCross.ViewModels;
using Newtonsoft.Json;
using WshLst.Core.Interfaces;
using WshLst.Core.Models;
using ZXing;
using ZXing.Mobile;

namespace WshLst.Core
{
	public class BarcodeApplicatonObject : MvxApplicationObject, IBarcodeScanner
	{
		public BarcodeApplicatonObject()
		{
		}

		private const string SCANDIT_API_KEY = "wsWII25QpvSvPOZ2cpEwr-w3fed5TiKKbxeNp9AwWr5";

		public Task<Result> Scan()
		{
#if MONOANDROID
			var activity = this.GetService<Cirrious.MvvmCross.Droid.Interfaces.IMvxAndroidCurrentTopActivity>();
			var scanner = new MobileBarcodeScanner(activity.Activity);
#else
			var scanner = new MobileBarcodeScanner();
#endif
			return scanner.Scan();
		}

		public void LookupProduct(string upc, Action<ScanditProduct> onComplete)
		{
			var wc = new WebClient();
			wc.DownloadStringCompleted += (s, e) =>
				{
					ScanditProduct product = null;

					try
					{
						product = JsonConvert.DeserializeObject<ScanditProduct>(e.Result);
					}
					catch (Exception)
					{
					}

					if (onComplete != null)
						onComplete(product);
				};

			try
			{
				wc.DownloadStringAsync(
					new Uri(string.Format("https://api.scandit.com/v1/products/{0}?key={1}", upc, SCANDIT_API_KEY)));
			}
			catch
			{
				if (onComplete != null)
					onComplete(null);
			}
		}
	}
}