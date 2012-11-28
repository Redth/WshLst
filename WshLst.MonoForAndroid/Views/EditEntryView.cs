using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Cirrious.MvvmCross.Binding.Droid.Views;
using Cirrious.MvvmCross.ExtensionMethods;
using WshLst.Core.ViewModels;
using WshLst.Core.Models;
using Xamarin.Media;

namespace WshLst.MonoForAndroid.Views
{
	[Activity(Label = "Wish List Item", Icon = "@drawable/icontransparent")]
	public class EditEntryView : MvxBindingActivityView<EditEntryViewModel>
	{
		Button buttonScan;
		ImageView imagePhoto;
		Button buttonAddPhoto;
		Button buttonRemovePhoto;
		Button buttonSave;
		Button buttonCancel;

		protected override void OnViewModelSet()
		{
			RequestWindowFeature(WindowFeatures.ActionBar);

			SetContentView(Resource.Layout.Page_EditEntryView);

			buttonScan = this.FindViewById<Button>(Resource.Id.buttonScanBarcode);

			imagePhoto = this.FindViewById<ImageView>(Resource.Id.imagePhoto);
			buttonAddPhoto = this.FindViewById<Button>(Resource.Id.buttonAddPhoto);
			buttonRemovePhoto = this.FindViewById<Button>(Resource.Id.buttonRemovePhoto);
			buttonSave = this.FindViewById<Button>(Resource.Id.buttonSave);
			buttonCancel = this.FindViewById<Button>(Resource.Id.buttonCancel);

			this.buttonRemovePhoto.Visibility = ViewStates.Gone;
			this.imagePhoto.Visibility = ViewStates.Gone;

			this.buttonScan.Click += (s, e) =>
			{
				this.ViewModel.Scan();
			};
			
			this.buttonAddPhoto.Click += (s, e) =>
			{
				this.ShowQuestion("Add Photo", "Would you like to Choose an existing photo or Take a New one?", "Take New", "Choose Existing",
					() => addPhoto(true), () => addPhoto(false));
			};

			this.buttonRemovePhoto.Click += (s, e) =>
			{
				this.ViewModel.RemovePhoto();
			};

			this.ViewModel.PropertyChanged += (s, e) =>
			{
				switch (e.PropertyName)
				{
					case "EntryImage":
						if (this.ViewModel.HasImage)
						{
							var converter = new Base64ToBitmapDrawableConverter();
							var drawable = (BitmapDrawable)converter.Convert(this.ViewModel.EntryImage.ImageBase64, typeof(BitmapDrawable), null, System.Globalization.CultureInfo.CurrentCulture);

							this.imagePhoto.SetImageDrawable(drawable);
						}
						break;
					case "HasImage":
						if (this.ViewModel.HasImage)
						{
							imagePhoto.Visibility = ViewStates.Visible;
							buttonAddPhoto.Visibility = ViewStates.Gone;
							buttonRemovePhoto.Visibility = ViewStates.Visible;
						}
						else
						{
							imagePhoto.Visibility = ViewStates.Gone;
							buttonAddPhoto.Visibility = ViewStates.Visible;
							buttonRemovePhoto.Visibility = ViewStates.Gone;
						}
						break;
				}
			};

			this.buttonCancel.Click += (s, e) =>
			{
				this.ViewModel.Cancel();
			};

			this.buttonSave.Click += (s, e) =>
			{
				this.ViewModel.Save();
			};

			this.ViewModel.LoadEntry();
		}

		void addPhoto(bool takeNew)
		{
			var mediaFileSource = this.GetService<Core.Interfaces.IMediaFileSource>();
			mediaFileSource.GetPhoto(takeNew).ContinueWith((t) =>
			{
				var ex = t.Exception;

				if (t.Status == System.Threading.Tasks.TaskStatus.RanToCompletion && t.Result != null)
				{
					var mediaFile = t.Result;
					byte[] imgData;
					byte[] buffer = new byte[1024];

					using (var ms = new System.IO.MemoryStream())
					using (var sr = mediaFile.GetStream())
					{
						int read = buffer.Length;
						while (read >= buffer.Length)
						{
							read = sr.Read(buffer, 0, buffer.Length);
							ms.Write(buffer, 0, read);
						}

						imgData = ms.ToArray();
					}

					var bmp = Android.Graphics.BitmapFactory.DecodeByteArray(imgData, 0, imgData.Length);

					this.ViewModel.AddPhoto(Convert.ToBase64String(imgData));

					mediaFile.Dispose();
				}
			});
		}
	}
}