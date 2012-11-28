using Android.App;
using Android.Views;
using Android.Widget;
using Cirrious.MvvmCross.Binding.Droid;
using Cirrious.MvvmCross.Binding.Droid.Views;
using Cirrious.MvvmCross.ExtensionMethods;
using Cirrious.MvvmCross.Interfaces.ServiceProvider;
using WshLst.Core;
using WshLst.Core.Interfaces;
using WshLst.Core.Models;
using WshLst.Core.ViewModels;

namespace WshLst.MonoForAndroid.Views
{
	[Activity(Label = "Wsh Lst - Login", Icon = "@drawable/icontransparent")]
	public class LoginView : MvxBindingActivityView<LoginViewModel>, IMvxServiceConsumer<IGeolocator>
	{
		protected override void OnViewModelSet()
		{
			RequestWindowFeature(WindowFeatures.ActionBar);

			SetContentView(Resource.Layout.Page_LoginView);
			
			var geo = this.GetService<IGeolocator>();
			geo.StartTracking();
		}

		protected override void OnStart()
		{
			base.OnStart();

			if (App.IsLaunch)
			{
				App.IsLaunch = false;

				ViewModel.CheckLogin();
			}
		}
	}
}