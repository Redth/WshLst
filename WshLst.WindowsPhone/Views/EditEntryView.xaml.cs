using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using WshLst.Core.ViewModels;
using Cirrious.MvvmCross.WindowsPhone.Views;
using Cirrious.MvvmCross.ExtensionMethods;
using Xamarin.Media;

namespace WshLst.Views
{
	public class BaseNewEntryView : MvxPhonePage<EditEntryViewModel> { }

	public partial class EditEntryView : BaseNewEntryView
	{
		public EditEntryView()
		{
			InitializeComponent();
		}

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);

			this.ViewModel.LoadEntry();


			this.ViewModel.PropertyChanged += ViewModel_PropertyChanged;		
		}

		void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (e.PropertyName.Equals("EntryImage") && this.ViewModel != null && this.ViewModel.EntryImage != null && !string.IsNullOrEmpty(this.ViewModel.EntryImage.ImageBase64))
			{
				this.Photo = new BitmapImage();

				byte[] bytes = Convert.FromBase64String(this.ViewModel.EntryImage.ImageBase64);
				using (var stream = new System.IO.MemoryStream(bytes))
				{
					this.Photo.SetSource(stream);
					this.photo.Source = this.Photo;
				}
			}
		}

		protected override void OnNavigatedFrom(NavigationEventArgs e)
		{
			this.ViewModel.PropertyChanged -= ViewModel_PropertyChanged;

			base.OnNavigatedFrom(e);
		}

		private void cancel_Click(object sender, EventArgs e)
		{
			this.ViewModel.Cancel();
		}

		private void save_Click(object sender, EventArgs e)
		{
			this.ViewModel.Save();
		}

		private void textbox_Changed(object sender, TextChangedEventArgs e)
		{
			((TextBox)sender).GetBindingExpression(TextBox.TextProperty).UpdateSource();
		}

		private void scanBarcode_Click(object sender, RoutedEventArgs e)
		{
			this.ViewModel.Scan();
		}

		System.Windows.Media.Imaging.BitmapImage Photo = null;
		
		private async void addPhoto(bool takeNew)
		{
			//Capture photo file from view
			var picker = new Xamarin.Media.MediaPicker();
			var options = new Xamarin.Media.StoreCameraMediaOptions();

			var mediaFileSource = this.GetService<Core.Interfaces.IMediaFileSource>();
			MediaFile mediaFile = null;

			try { mediaFile = await mediaFileSource.GetPhoto(takeNew); }
			catch { }

			if (mediaFile != null && !string.IsNullOrEmpty(mediaFile.Path))
			{
				this.Dispatcher.BeginInvoke(() =>
				{
					
					this.Photo = new System.Windows.Media.Imaging.BitmapImage();
					this.Photo.SetSource(mediaFile.GetStream());

					
					var photoBase64 = string.Empty;
					var wbmp = new System.Windows.Media.Imaging.WriteableBitmap(this.Photo);
										
					using (var ms = new System.IO.MemoryStream())
					{
						wbmp.SaveJpeg(ms, 640, 480, 0, 60);
						photoBase64 = Convert.ToBase64String(ms.ToArray());
					}
					
					this.ViewModel.AddPhoto(photoBase64);

					//Clean up!
					mediaFile.Dispose();
				}); 
			}
		}

		private void removePhoto_Click(object sender, RoutedEventArgs e)
		{
			this.photo.Source = null;
			this.Photo = null;
			this.ViewModel.RemovePhoto();
		}

		private void choosePhoto_Click(object sender, RoutedEventArgs e)
		{
			addPhoto(false);
		}

		private void takePhoto_Click(object sender, RoutedEventArgs e)
		{
			addPhoto(true);
		}
	}
}