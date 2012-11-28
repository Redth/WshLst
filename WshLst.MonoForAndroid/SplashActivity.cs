﻿using Android.App;
using Cirrious.MvvmCross.Droid.Platform;
using Cirrious.MvvmCross.Droid.Views;

namespace WshLst.MonoForAndroid
{
	[Activity(Label = "Wsh Lst", MainLauncher = true, NoHistory = true, Icon = "@drawable/icontransparent")]
	public class SplashScreenActivity
		: MvxBaseSplashScreenActivity
	{
		public SplashScreenActivity()
			: base(Resource.Layout.SplashScreen)
		{
		}
	}
}