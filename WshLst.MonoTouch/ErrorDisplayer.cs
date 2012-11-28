using Cirrious.MvvmCross.ExtensionMethods;
using Cirrious.MvvmCross.Interfaces.ServiceProvider;
using MonoTouch.UIKit;
using WshLst.Core.Interfaces;

namespace WshLst.MonoTouch
{
	public class ErrorDisplayer
		: IMvxServiceConsumer<IErrorSource>
	{
		public ErrorDisplayer()
		{
			var source = this.GetService<IErrorSource>();
			source.ErrorReported += (sender, args) => ShowError(args.Message);
		}

		private void ShowError(string message)
		{
			var errorView = new UIAlertView("Wsh Lst", message, null, "OK", null);
			errorView.Show();
		}
	}
}