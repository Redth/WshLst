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

using Cirrious.MvvmCross.Application;
using Cirrious.MvvmCross.Binding.Droid;
using Cirrious.MvvmCross.Plugins.Visibility;

namespace WshLst.MonoForAndroid
{
	public class Setup
		: MvxBaseAndroidBindingSetup
	{
		public Setup(Context applicationContext)
			: base(applicationContext)
		{
		}

		protected override MvxApplication CreateApp()
		{
			return new WshLst.Core.App();
		}

		public class Converters
		{
			public readonly MvxVisibilityConverter Visibility = new MvxVisibilityConverter();
		}

		protected override IEnumerable<Type> ValueConverterHolders
		{
			get { return new[] { typeof(Converters) }; }
		}

		protected override void InitializeLastChance()
		{
			var errorHandler = new ErrorDisplayer(ApplicationContext);
			Cirrious.MvvmCross.Plugins.Visibility.PluginLoader.Instance.EnsureLoaded();
			base.InitializeLastChance();
		}
	}
}