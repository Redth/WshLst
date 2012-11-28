using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Cirrious.MvvmCross.Droid.Views;
using Cirrious.MvvmCross.ExtensionMethods;
using Cirrious.MvvmCross.Binding.Droid.Views;
using Cirrious.MvvmCross.Interfaces.ServiceProvider;
using WshLst.Core.ViewModels;
using WshLst.Core.Models;

namespace WshLst.MonoForAndroid.Views
{
	[Activity(Label = "Wsh Lst - Login", Icon = "@drawable/icontransparent")]
	public class LoginView : MvxBindingActivityView<LoginViewModel>, IMvxServiceConsumer<WshLst.Core.Interfaces.IGeolocator>
	{
		ListView list;

		protected override void OnViewModelSet()
		{
			RequestWindowFeature(WindowFeatures.ActionBar);

			SetContentView(Resource.Layout.Page_LoginView);

			list = this.FindViewById<ListView>(Resource.Id.mvxList);

			list.ItemClick += (s, e) =>
			{
				var item = list.Adapter.GetItem(e.Position);

				var castItem = (Cirrious.MvvmCross.Binding.Droid.MvxJavaContainer)item;

				this.ViewModel.Login((WshLst.Core.Models.LoginPlatform)castItem.Object);
			};

			var geo = this.GetService<WshLst.Core.Interfaces.IGeolocator>();
			geo.StartTracking();
		}

		protected override void OnStart()
		{
			base.OnStart();

			if (WshLst.Core.App.IsLaunch)
			{
				WshLst.Core.App.IsLaunch = false;

				this.ViewModel.CheckLogin();
			}
		}
	}
}