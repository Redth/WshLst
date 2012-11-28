using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using Android.App;
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
		private ImageView _imagePhoto;

		protected override void OnViewModelSet()
		{
			RequestWindowFeature(WindowFeatures.ActionBar);

			SetContentView(Resource.Layout.Page_EditEntryView);

			_imagePhoto = FindViewById<ImageView>(Resource.Id.imagePhoto);
			_buttonAddPhoto = FindViewById<Button>(Resource.Id.buttonAddPhoto);

			_buttonAddPhoto.Click +=
				(s, e) =>
				this.ShowQuestion("Add Photo", "Would you like to Choose an existing photo or Take a New one?", "Take New",
				                  "Choose Existing",
				                  () => AddPhoto(true), () => AddPhoto(false));

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
					}
				};

			ViewModel.LoadEntry();
		}

		private void AddPhoto(bool takeNew)
		{
			var mediaFileSource = this.GetService<IMediaFileSource>();
			mediaFileSource.GetPhoto(takeNew).ContinueWith(t =>
				{
					var ex = t.Exception;

					if (t.Status != TaskStatus.RanToCompletion || t.Result == null) return;

					using (var mediaFile = t.Result)
					{
						byte[] imgData;

						using (var ms = new MemoryStream())
						using (var sc = mediaFile.GetStream())
						{
							sc.CopyTo(ms);
							imgData = ms.ToArray();
						}

						ViewModel.AddPhoto(Convert.ToBase64String(imgData));
					}
				});
		}
	}
}