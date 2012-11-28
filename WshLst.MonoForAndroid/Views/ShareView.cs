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
using Cirrious.MvvmCross.Binding.Droid;
using Cirrious.MvvmCross.Binding.Droid.Views;
using WshLst.Core.ViewModels;
using WshLst.Core.Models;

namespace WshLst.MonoForAndroid.Views
{
	[Activity(Label = "Share Wish List", Icon = "@drawable/icontransparent")]
	public class ShareView : MvxBindingActivityView<ShareViewModel>
	{
		ListView list;
		Button buttonSave;
		Button buttonCancel;

		protected override void OnStart()
		{
			base.OnStart();

			this.ViewModel.LoadContacts();
		}

		protected override void OnViewModelSet()
		{
			RequestWindowFeature(WindowFeatures.ActionBar);

			SetContentView(Resource.Layout.Page_ShareView);

			buttonSave = this.FindViewById<Button>(Resource.Id.buttonSave);
			buttonCancel = this.FindViewById<Button>(Resource.Id.buttonCancel);
			list = this.FindViewById<ListView>(Resource.Id.mvxList);

			this.list.FastScrollEnabled = true;

			buttonSave.Click += (s, e) =>
			{
				//Get emails
				var to = this.ViewModel.GetEmailTo();
				var subject = this.ViewModel.GetEmailSubject();
				var body = this.ViewModel.GetEmailBody();

				//Send email
				var emailIntent = new Intent(Intent.ActionSend);

				emailIntent.SetType("message/rfc822");

				if (!string.IsNullOrEmpty(to))
				{
					var toList = to.Split(';');
					emailIntent.PutExtra(Intent.ExtraEmail, toList);
				}

				emailIntent.PutExtra(Intent.ExtraSubject, subject);
				emailIntent.PutExtra(Intent.ExtraText, body);

				try
				{
					StartActivityForResult(emailIntent, 101);
				}
				catch
				{
					this.ShowInformation("Email Setup", "Please setup your Email on the device before sharing your Wish List!", "OK", null);
				}
			};

			buttonCancel.Click += (s, e) =>
			{
				this.ViewModel.Cancel();
			};			
		}

		protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
		{
			base.OnActivityResult(requestCode, resultCode, data);

			if (requestCode == 101)
				this.ViewModel.Finished();
		}
		
	}
}