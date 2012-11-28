using Cirrious.MvvmCross.Application;
using Cirrious.MvvmCross.Plugins.Json;
using Cirrious.MvvmCross.WindowsPhone.Platform;
using Microsoft.Phone.Controls;

namespace WshLst
{
	public class Setup : MvxBaseWindowsPhoneSetup
	{
		public Setup(PhoneApplicationFrame rootFrame) : base(rootFrame)
		{
		}

		protected override MvxApplication CreateApp()
		{
			var app = new Core.App();
			return app;
		}

		protected override void InitializeDefaultTextSerializer()
		{
			PluginLoader.Instance.EnsureLoaded();
		}
	}
}