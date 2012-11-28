using System;
using System.Collections.Generic;
using System.Linq;
using Cirrious.MvvmCross.ExtensionMethods;
using Cirrious.MvvmCross.Interfaces.ServiceProvider;
using Cirrious.MvvmCross.Interfaces.ViewModels;
using Cirrious.MvvmCross.Touch.Platform;
using Cirrious.MvvmCross.Touch.Views.Presenters;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace WshLst.MonoTouch
{
	[Register ("AppDelegate")]
	public partial class AppDelegate : MvxApplicationDelegate, IMvxServiceConsumer
	{
		// class-level declarations
		UIWindow window;
			
		public override bool FinishedLaunching(UIApplication app, NSDictionary options)
		{
			window = new UIWindow(UIScreen.MainScreen.Bounds);

			var presenter = new MvxTouchViewPresenter(this, window);
			var s = new Setup(this, presenter);
			s.Initialize();

			// start the app
			var start = this.GetService<IMvxStartNavigation>();
			start.Start();

			//Start Geo location tracking
			var geo = this.GetService<WshLst.Core.Interfaces.IGeolocator>();
			geo.StartTracking();
		
			// make the window visible
			window.MakeKeyAndVisible();
			
			return true;
		}

		public override void WillTerminate(UIApplication application)
		{
			var geo = this.GetService<WshLst.Core.Interfaces.IGeolocator>();
			geo.StopTracking();

			base.WillTerminate(application);
		}
	}
}

