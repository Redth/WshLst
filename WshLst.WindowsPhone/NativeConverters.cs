using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace WshLst.NativeConverters
{
	public class BooleanToVisibilityConverter : System.Windows.Data.IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null)
				return Visibility.Collapsed;

			var isVisible = (bool)value;

			return isVisible ? Visibility.Visible : Visibility.Collapsed;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var visiblity = (Visibility)value;

			return visiblity == Visibility.Visible;
		}
	}

	public class InvertedBooleanToVisibilityConverter : System.Windows.Data.IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null)
				return Visibility.Visible;

			var isVisible = (bool)value;

			return isVisible ? Visibility.Collapsed : Visibility.Visible;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var visiblity = (Visibility)value;

			return visiblity == Visibility.Collapsed;
		}
	}

	public class Base64ImageConverter : System.Windows.Data.IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			var photo = new System.Windows.Media.Imaging.BitmapImage();

			byte[] bytes = System.Convert.FromBase64String((string)value);
			using (var stream = new System.IO.MemoryStream(bytes))
			{
				photo.SetSource(stream);
				return photo;
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			var photoBase64 = string.Empty;

			var wbmp = new System.Windows.Media.Imaging.WriteableBitmap((System.Windows.Media.Imaging.BitmapSource)value);

			//this.photo.Source
			using (var ms = new System.IO.MemoryStream())
			{
				wbmp.SaveJpeg(ms, 640, 480, 0, 60);

				return System.Convert.ToBase64String(ms.ToArray());
			}
		}
	}


	public class StringToVisibilityConverter : System.Windows.Data.IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (string.IsNullOrEmpty((string)value))
				return System.Windows.Visibility.Collapsed;
			else
				return System.Windows.Visibility.Visible;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotSupportedException();
		}
	}
}
