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
		
			try
			{
				using (var mediaFile = await mediaFileSource.GetPhoto(takeNew))
				{
					ViewModel.AddPhoto(mediaFile.GetStream());
				}
			}
			catch (Exception)
			{
			}

			/*if (mediaFile != null && !string.IsNullOrEmpty(mediaFile.Path))
			{
				Dispatcher.BeginInvoke(() =>
					{
						ViewModel.AddPhoto(mediaFile.GetStream());
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
			}*/
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