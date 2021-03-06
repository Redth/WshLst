using System;
using System.Collections.Generic;
using Cirrious.MvvmCross.Application;
using Cirrious.MvvmCross.Dialog.Touch;
using Cirrious.MvvmCross.Platform;
using Cirrious.MvvmCross.Plugins.Visibility;
using Cirrious.MvvmCross.Touch.Interfaces;
using Cirrious.MvvmCross.Touch.Platform;

namespace WshLst.MonoTouch
{
	public class Setup : MvxTouchDialogBindingSetup
	{
		public Setup(MvxApplicationDelegate applicationDelegate, IMvxTouchViewPresenter presenter)
			: base(applicationDelegate, presenter)
		{
		}

		protected override MvxApplication CreateApp()
		{
			var app = new WshLst.Core.App();
			return app;
		}

		public class Converters
		{
			public readonly MvxVisibilityConverter Visibility = new MvxVisibilityConverter();
			public readonly MvxInvertedVisibilityConverter InvertedVisibility = new MvxInvertedVisibilityConverter();
			public readonly Base64ToUIImageConverter Base64ToUIImage = new Base64ToUIImageConverter(); 
		}

		protected override IEnumerable<Type> ValueConverterHolders
		{
			get { return new[] { typeof(Converters) }; }
		}

		protected override void InitializeLastChance()
		{
			Cirrious.MvvmCross.Plugins.Visibility.PluginLoader.Instance.EnsureLoaded();

			// create a new error displayer - it will hook itself into the framework
			var errorDisplayer = new ErrorDisplayer();

			base.InitializeLastChance();
		}

		protected override void AddPluginsLoaders(MvxLoaderPluginRegistry registry)
		{
			registry.AddConventionalPlugin<Cirrious.MvvmCross.Plugins.Visibility.Touch.Plugin>();
			registry.AddConventionalPlugin<Cirrious.MvvmCross.Plugins.DownloadCache.Touch.Plugin>();
			registry.AddConventionalPlugin<Cirrious.MvvmCross.Plugins.File.Touch.Plugin>();
			base.AddPluginsLoaders(registry);
		}
	}
}
