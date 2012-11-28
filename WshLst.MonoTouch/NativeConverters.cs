using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Cirrious.MvvmCross.Converters;
using Cirrious.MvvmCross.ExtensionMethods;
using Cirrious.MvvmCross.Interfaces.ServiceProvider;

using MonoTouch.CoreFoundation;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace WshLst.MonoTouch
{
	public class Base64ToUIImageConverter : MvxBaseValueConverter
	{
		public override object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			var img = (UIImage)value;

			return System.Convert.ToBase64String(img.AsJPEG().ToArray());
		}

		public override object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			string base64 = (string)value;

			var bytes = System.Convert.FromBase64String(base64);

			return UIImage.LoadFromData(NSData.FromArray(bytes));
		}
	}

}