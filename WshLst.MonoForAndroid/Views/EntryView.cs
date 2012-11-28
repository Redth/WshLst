using System.Globalization;
using Android.App;
using Android.Graphics.Drawables;
using Android.Views;
using Android.Widget;
using Cirrious.MvvmCross.Binding.Droid.Views;
using WshLst.Core.ViewModels;

namespace WshLst.MonoForAndroid.Views
{
	[Activity(Label = "Wish List Item", Icon = "@drawable/icontransparent")]
	public class EntryView : MvxBindingActivityView<EntryViewModel>
	{
		private ImageView _imagePhoto;

		protected override void OnStart()
		{
			base.OnStart();

			ViewModel.LoadEntry();
		}

		protected override void OnViewModelSet()
		{
			RequestWindowFeature(WindowFeatures.ActionBar);

			SetContentView(Resource.Layout.Page_EntryView);

			_imagePhoto = FindViewById<ImageView>(Resource.Id.imagePhoto);

			ViewModel.PropertyChanged += (s, e) =>
				{
					if (e.PropertyName.Equals("EntryImage"))
					{
						if (ViewModel.HasImage)
						{
							var converter = new Base64ToBitmapDrawableConverter();
							var drawable =
								(BitmapDrawable)
								converter.Convert(ViewModel.EntryImage.ImageBase64, typeof (BitmapDrawable), null, CultureInfo.CurrentCulture);

							_imagePhoto.SetImageDrawable(drawable);
						}
					}
				};

			ViewModel.LoadEntry();
		}
	}
}