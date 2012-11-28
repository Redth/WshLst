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

using Cirrious.MvvmCross.Converters;
using Cirrious.MvvmCross.ExtensionMethods;
using Cirrious.MvvmCross.Interfaces.ServiceProvider;

namespace WshLst.MonoForAndroid
{
	public class Base64ToBitmapDrawableConverter : MvxBaseValueConverter  
	{
		public override object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			string base64 = string.Empty;

			var bitmapDrawable = (Android.Graphics.Drawables.BitmapDrawable)value;
			using (var ms = new System.IO.MemoryStream())
			{
				bitmapDrawable.Bitmap.Compress(Android.Graphics.Bitmap.CompressFormat.Jpeg, 70, ms);

				System.Convert.ToBase64String(ms.ToArray());
			}

			return base64;
		}

		public override object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			string base64 = (string)value;

			var bytes = System.Convert.FromBase64String(base64);

			var drawable = Android.Graphics.BitmapFactory.DecodeByteArray(bytes, 0, bytes.Length);

			return new Android.Graphics.Drawables.BitmapDrawable(drawable);
		}
	}

}