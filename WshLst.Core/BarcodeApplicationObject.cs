using System;
using System.Threading.Tasks;
using System.Net;
using Cirrious.MvvmCross.Application;
using Cirrious.MvvmCross.Core;
using Cirrious.MvvmCross.ExtensionMethods;
using Cirrious.MvvmCross.Interfaces;
using Cirrious.MvvmCross.ViewModels;
using WshLst.Core.Interfaces;
using ZXing;
using ZXing.Mobile;

namespace WshLst.Core
{
	public class BarcodeApplicatonObject : MvxApplicationObject, IBarcodeScanner
	{
		const string SCANDIT_API_KEY = "wsWII25QpvSvPOZ2cpEwr-w3fed5TiKKbxeNp9AwWr5";

		public BarcodeApplicatonObject()
		{
		}

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

		public void LookupProduct(string upc, Action<Models.ScanditProduct> onComplete)
		{
			var wc = new WebClient();
			wc.DownloadStringCompleted += (s, e) => 
			{
				Models.ScanditProduct product = null;

				try { product =	Newtonsoft.Json.JsonConvert.DeserializeObject<Models.ScanditProduct>(e.Result); }
				catch { }

				if (onComplete != null)
					onComplete(product);
			};

			string data = string.Empty;

			try 
			{	
				wc.DownloadStringAsync(new Uri(string.Format("https://api.scandit.com/v1/products/{0}?key={1}", upc, SCANDIT_API_KEY)));
			}
			catch 
			{
				if (onComplete != null)
					onComplete(null);
			}
		}
	}
}