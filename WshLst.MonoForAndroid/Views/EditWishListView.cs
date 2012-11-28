using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Cirrious.MvvmCross.Binding.Droid.Views;
using WshLst.Core.ViewModels;
using WshLst.Core.Models;

namespace WshLst.MonoForAndroid.Views
{
	[Activity(Label = "New Wish List", Icon = "@drawable/icontransparent")]
	public class EditWishListView : MvxBindingActivityView<EditWishListViewModel>
	{
		EditText textName;
		EditText textDescription;
		Button buttonSave;
		Button buttonCancel;

		protected override void OnViewModelSet()
		{
			RequestWindowFeature(WindowFeatures.ActionBar);

			SetContentView(Resource.Layout.Page_EditWishListView);

			textName = this.FindViewById<EditText>(Resource.Id.textName);
			textDescription = this.FindViewById<EditText>(Resource.Id.textDescription);
			buttonCancel = this.FindViewById<Button>(Resource.Id.buttonCancel);
			buttonSave = this.FindViewById<Button>(Resource.Id.buttonSave);
			

			buttonCancel.Click += (s, e) => { this.ViewModel.Cancel(); };

			buttonSave.Click += (s, e) => 
			{
				this.ViewModel.Name = this.textName.Text;
				this.ViewModel.Description = this.textDescription.Text;
				
				this.ViewModel.Save();
			};

			this.ViewModel.PropertyChanged += (s, e) =>
			{
				switch (e.PropertyName)
				{
					case "Name":
						textName.Text = this.ViewModel.Name;
						break;
					case "Description":
						textDescription.Text = this.ViewModel.Description;
						break;
					case "ViewTitle":
						this.Title = this.ViewModel.ViewTitle;
						break;
				}
			};

			

		}
	}
}