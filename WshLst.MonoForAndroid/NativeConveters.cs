using System;
using System.Globalization;
using System.IO;
using Android.Graphics;
using Android.Graphics.Drawables;
using Cirrious.MvvmCross.Converters;

namespace WshLst.MonoForAndroid
{
	public class Base64ToBitmapDrawableConverter : MvxBaseValueConverter
	{
		public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var bitmapDrawable = (BitmapDrawable) value;
			using (var ms = new MemoryStream())
			{
				bitmapDrawable.Bitmap.Compress(Bitmap.CompressFormat.Jpeg, 70, ms);

				return System.Convert.ToBase64String(ms.ToArray());
			}
		}

		public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var base64 = (string) value;

			var bytes = System.Convert.FromBase64String(base64);

			var drawable = BitmapFactory.DecodeByteArray(bytes, 0, bytes.Length);

			return new BitmapDrawable(drawable);
		}
	}
}