﻿using Cirrious.MvvmCross.Application;
using Cirrious.MvvmCross.Platform;
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
			var app = new WshLst.Core.App();
			return app;
		}

		protected override void InitializeDefaultTextSerializer()
		{
			Cirrious.MvvmCross.Plugins.Json.PluginLoader.Instance.EnsureLoaded(true);
		}
	}
}