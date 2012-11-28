using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Cirrious.MvvmCross.ExtensionMethods;
using Cirrious.MvvmCross.WindowsPhone.Views;
using WshLst.Core.Interfaces;
using WshLst.Core.ViewModels;
using Xamarin.Media;

namespace WshLst.Views
{
	public class BaseNewEntryView : MvxPhonePage<EditEntryViewModel>
	{
	}

	public partial class EditEntryView : BaseNewEntryView
	{
		private BitmapImage _photo;

		public EditEntryView()
		{
			InitializeComponent();
		}

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);

			ViewModel.LoadEntry();

			ViewModel.PropertyChanged += ViewModel_PropertyChanged;
		}

		private void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName.Equals("EntryImage") && ViewModel != null && ViewModel.EntryImage != null &&
			    !string.IsNullOrEmpty(ViewModel.EntryImage.ImageBase64))
			{
				_photo = new BitmapImage();

				byte[] bytes = Convert.FromBase64String(ViewModel.EntryImage.ImageBase64);
				using (var stream = new MemoryStream(bytes))
				{
					_photo.SetSource(stream);
					photo.Source = _photo;
				}
			}
		}

		protected override void OnNavigatedFrom(NavigationEventArgs e)
		{
			ViewModel.PropertyChanged -= ViewModel_PropertyChanged;

			base.OnNavigatedFrom(e);
		}

		private void cancel_Click(object sender, EventArgs e)
		{
			ViewModel.Cancel();
		}

		private void save_Click(object sender, EventArgs e)
		{
			ViewModel.Save();
		}

		private void textbox_Changed(object sender, TextChangedEventArgs e)
		{
			((TextBox) sender).GetBindingExpression(TextBox.TextProperty).UpdateSource();
		}

		private void scanBarcode_Click(object sender, RoutedEventArgs e)
		{
			ViewModel.Scan();
		}

		private async void AddPhoto(bool takeNew)
		{
			//Capture photo file from view
			var mediaFileSource = this.GetService<IMediaFileSource>();
			MediaFile mediaFile = null;

			try
			{
				mediaFile = await mediaFileSource.GetPhoto(takeNew);
			}
			catch (Exception)
			{
			}

			if (mediaFile != null && !string.IsNullOrEmpty(mediaFile.Path))
			{
				Dispatcher.BeginInvoke(() =>
					{
						_photo = new BitmapImage();
						_photo.SetSource(mediaFile.GetStream());


						var photoBase64 = string.Empty;
						var wbmp = new WriteableBitmap(_photo);

						using (var ms = new MemoryStream())
						{
							wbmp.SaveJpeg(ms, 640, 480, 0, 60);
							photoBase64 = Convert.ToBase64String(ms.ToArray());
						}

						ViewModel.AddPhoto(photoBase64);

						//Clean up!
						mediaFile.Dispose();
					});
			}
		}

		private void removePhoto_Click(object sender, RoutedEventArgs e)
		{
			photo.Source = null;
			_photo = null;
			ViewModel.RemovePhoto();
		}

		private void choosePhoto_Click(object sender, RoutedEventArgs e)
		{
			AddPhoto(false);
		}

		private void takePhoto_Click(object sender, RoutedEventArgs e)
		{
			AddPhoto(true);
		}
	}
}