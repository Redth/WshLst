using System;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace WshLst.NativeConverters
{
	public class BooleanToVisibilityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null)
				return Visibility.Collapsed;

			var isVisible = (bool) value;

			return isVisible ? Visibility.Visible : Visibility.Collapsed;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var visiblity = (Visibility) value;

			return visiblity == Visibility.Visible;
		}
	}

	public class InvertedBooleanToVisibilityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null)
				return Visibility.Visible;

			var isVisible = (bool) value;

			return isVisible ? Visibility.Collapsed : Visibility.Visible;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var visiblity = (Visibility) value;

			return visiblity == Visibility.Collapsed;
		}
	}

	public class Base64ImageConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var photo = new BitmapImage();

			byte[] bytes = System.Convert.FromBase64String((string) value);

			if (bytes == null || bytes.Length <= 0)
				return photo;

			using (var stream = new MemoryStream(bytes))
			{
				photo.SetSource(stream);
				return photo;
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var wbmp = new WriteableBitmap((BitmapSource) value);

			using (var ms = new MemoryStream())
			{
				wbmp.SaveJpeg(ms, 640, 480, 0, 60);

				return System.Convert.ToBase64String(ms.ToArray());
			}
		}
	}

	public class StringToVisibilityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return string.IsNullOrEmpty((string) value) ? Visibility.Collapsed : Visibility.Visible;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}
	}
}