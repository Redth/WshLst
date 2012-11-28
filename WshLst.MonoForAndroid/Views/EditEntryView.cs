using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using Android.App;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Views;
using Android.Widget;
using Cirrious.MvvmCross.Binding.Droid.Views;
using Cirrious.MvvmCross.ExtensionMethods;
using WshLst.Core.Interfaces;
using WshLst.Core.ViewModels;

namespace WshLst.MonoForAndroid.Views
{
	[Activity(Label = "Wish List Item", Icon = "@drawable/icontransparent")]
	public class EditEntryView : MvxBindingActivityView<EditEntryViewModel>
	{
		private Button _buttonAddPhoto;
		private Button _buttonCancel;
		private Button _buttonRemovePhoto;
		private Button _buttonSave;
		private Button _buttonScan;
		private ImageView _imagePhoto;

		protected override void OnViewModelSet()
		{
			RequestWindowFeature(WindowFeatures.ActionBar);

			SetContentView(Resource.Layout.Page_EditEntryView);

			_buttonScan = FindViewById<Button>(Resource.Id.buttonScanBarcode);

			_imagePhoto = FindViewById<ImageView>(Resource.Id.imagePhoto);
			_buttonAddPhoto = FindViewById<Button>(Resource.Id.buttonAddPhoto);
			_buttonRemovePhoto = FindViewById<Button>(Resource.Id.buttonRemovePhoto);
			_buttonSave = FindViewById<Button>(Resource.Id.buttonSave);
			_buttonCancel = FindViewById<Button>(Resource.Id.buttonCancel);

			_buttonRemovePhoto.Visibility = ViewStates.Gone;
			_imagePhoto.Visibility = ViewStates.Gone;

			_buttonScan.Click += (s, e) => ViewModel.Scan();

			_buttonAddPhoto.Click += (s, e) => this.ShowQuestion("Add Photo", "Would you like to Choose an existing photo or Take a New one?", "Take New",
			                                                     "Choose Existing",
			                                                     () => AddPhoto(true), () => AddPhoto(false));

			_buttonRemovePhoto.Click += (s, e) => ViewModel.RemovePhoto();

			ViewModel.PropertyChanged += (s, e) =>
				{
					switch (e.PropertyName)
					{
						case "EntryImage":
							if (ViewModel.HasImage)
							{
								var converter = new Base64ToBitmapDrawableConverter();
								var drawable =
									(BitmapDrawable)
									converter.Convert(ViewModel.EntryImage.ImageBase64, typeof (BitmapDrawable), null, CultureInfo.CurrentCulture);

								_imagePhoto.SetImageDrawable(drawable);
							}
							break;
						case "HasImage":
							if (ViewModel.HasImage)
							{
								_imagePhoto.Visibility = ViewStates.Visible;
								_buttonAddPhoto.Visibility = ViewStates.Gone;
								_buttonRemovePhoto.Visibility = ViewStates.Visible;
							}
							else
							{
								_imagePhoto.Visibility = ViewStates.Gone;
								_buttonAddPhoto.Visibility = ViewStates.Visible;
								_buttonRemovePhoto.Visibility = ViewStates.Gone;
							}
							break;
					}
				};

			_buttonCancel.Click += (s, e) => ViewModel.Cancel();

			_buttonSave.Click += (s, e) => ViewModel.Save();

			ViewModel.LoadEntry();
		}

		private void AddPhoto(bool takeNew)
		{
			var mediaFileSource = this.GetService<IMediaFileSource>();
			mediaFileSource.GetPhoto(takeNew).ContinueWith(t =>
				{
					var ex = t.Exception;

					if (t.Status != TaskStatus.RanToCompletion || t.Result == null) return;

					var mediaFile = t.Result;
					byte[] imgData;
					var buffer = new byte[1024];

					using (var ms = new MemoryStream())
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

					//var bmp = BitmapFactory.DecodeByteArray(imgData, 0, imgData.Length);

					ViewModel.AddPhoto(Convert.ToBase64String(imgData));

					mediaFile.Dispose();
				});
		}
	}
}