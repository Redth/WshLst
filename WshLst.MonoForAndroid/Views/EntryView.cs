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
using Cirrious.MvvmCross.Binding.Droid;
using Cirrious.MvvmCross.Binding.Droid.Views;
using WshLst.Core.ViewModels;
using WshLst.Core.Models;

namespace WshLst.MonoForAndroid.Views
{
	[Activity(Label = "Wish List Item", Icon = "@drawable/icontransparent")]
	public class EntryView : MvxBindingActivityView<EntryViewModel>
	{
		ImageView imagePhoto;

		protected override void OnStart()
		{
			base.OnStart();

			this.ViewModel.LoadEntry();
		}

		protected override void OnViewModelSet()
		{
			RequestWindowFeature(WindowFeatures.ActionBar);

			SetContentView(Resource.Layout.Page_EntryView);

			this.imagePhoto = this.FindViewById<ImageView>(Resource.Id.imagePhoto);

			this.ViewModel.PropertyChanged += (s, e) =>
			{
				if (e.PropertyName.Equals("EntryImage"))
				{
					if (this.ViewModel.HasImage)
					{
						var converter = new Base64ToBitmapDrawableConverter();
						var drawable = (BitmapDrawable)converter.Convert(this.ViewModel.EntryImage.ImageBase64, typeof(BitmapDrawable), null, System.Globalization.CultureInfo.CurrentCulture);

						this.imagePhoto.SetImageDrawable(drawable);
					}
				}
			};

			this.ViewModel.LoadEntry();
		}
	}
}