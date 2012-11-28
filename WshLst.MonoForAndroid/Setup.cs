using System;
using System.Collections.Generic;
using Android.Content;
using Cirrious.MvvmCross.Application;
using Cirrious.MvvmCross.Binding.Droid;
using Cirrious.MvvmCross.Plugins.Visibility;
using WshLst.Core;

namespace WshLst.MonoForAndroid
{
	public class Setup
		: MvxBaseAndroidBindingSetup
	{
		public Setup(Context applicationContext)
			: base(applicationContext)
		{
		}

		protected override IEnumerable<Type> ValueConverterHolders
		{
			get { return new[] {typeof (Converters)}; }
		}

		protected override MvxApplication CreateApp()
		{
			return new App();
		}

		protected override void InitializeLastChance()
		{
			var errorHandler = new ErrorDisplayer(ApplicationContext);
			PluginLoader.Instance.EnsureLoaded();
			base.InitializeLastChance();
		}

		public class Converters
		{
			public readonly MvxVisibilityConverter Visibility = new MvxVisibilityConverter();
		}
	}
}