using Cirrious.MvvmCross.Application;
using Cirrious.MvvmCross.ExtensionMethods;
using Cirrious.MvvmCross.Interfaces.ServiceProvider;
using Cirrious.MvvmCross.Interfaces.ViewModels;
using Microsoft.WindowsAzure.MobileServices;
using WshLst.Core.Interfaces;

namespace WshLst.Core
{
	public class App : MvxApplication,
	    IMvxServiceProducer<IMvxStartNavigation>,
	    IMvxServiceProducer<IErrorReporter>,
	    IMvxServiceProducer<IErrorSource>,
	    IMvxServiceProducer<ISettingsProvider>,
	    IMvxServiceProducer<IGeolocator>,
	    IMvxServiceProducer<IBarcodeScanner>,
	    IMvxServiceProducer<IMediaFileSource>,
		IMvxServiceProducer<IAddressBookSource>
	{
		public static bool IsLaunch = true;

		public static MobileServiceClient Azure;

		public App()
		{
			Azure = new MobileServiceClient(Config.AZURE_MOBILE_SERVICE_URL, Config.AZURE_MOBILE_SERVICE_APPKEY);

			var errAppObj = new ErrorApplicationObject();
			var settings = new Settings();
			settings.Load();

			this.RegisterServiceInstance<IMvxStartNavigation>(new StartApplicationObject());
			this.RegisterServiceInstance<IErrorSource>(errAppObj);
			this.RegisterServiceInstance<IErrorReporter>(errAppObj);
			this.RegisterServiceInstance<ISettingsProvider>(settings);
			this.RegisterServiceInstance<IMediaFileSource>(new MediaFileApplicationObject());

			var geo = new GeolocatorApplicationObject();
			geo.StartTracking();

			this.RegisterServiceInstance<IGeolocator>(geo);
			this.RegisterServiceInstance<IBarcodeScanner>(new BarcodeApplicatonObject());
			this.RegisterServiceInstance<IAddressBookSource>(new AddressBookApplicationObject());
		}

		public static void Logout()
		{
			Azure = new MobileServiceClient(Config.AZURE_MOBILE_SERVICE_URL, Config.AZURE_MOBILE_SERVICE_APPKEY);
		}
	}
}