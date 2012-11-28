using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

using Cirrious.MvvmCross.Droid.Interfaces;
using Cirrious.MvvmCross.Binding.Droid.Interfaces.Views;
using Cirrious.MvvmCross.ExtensionMethods;
using Cirrious.MvvmCross.Interfaces.ServiceProvider;
using WshLst.Core.Interfaces;

namespace WshLst
{
	public class ErrorDisplayer
		: IMvxServiceConsumer<IErrorSource>
		  , IMvxServiceConsumer<IMvxAndroidCurrentTopActivity>
	{
		private readonly Context _applicationContext;

		public ErrorDisplayer(Context applicationContext)
		{
			_applicationContext = applicationContext;

			var source = this.GetService<IErrorSource>();
			source.ErrorReported += (sender, args) => ShowError(args.Message);
		}

		private void ShowError(string message)
		{
			var activity = this.GetService<IMvxAndroidCurrentTopActivity>().Activity; //.Activity as IMvxBindingActivity;

			var ab = new AlertDialog.Builder(activity);

			ab.SetTitle("Error");
			ab.SetMessage(message);
			ab.Show();
		}
	}
}