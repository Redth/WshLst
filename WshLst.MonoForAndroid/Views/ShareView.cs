using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using Cirrious.MvvmCross.Binding.Droid.Views;
using WshLst.Core.ViewModels;

namespace WshLst.MonoForAndroid.Views
{
	[Activity(Label = "Share Wish List", Icon = "@drawable/icontransparent")]
	public class ShareView : MvxBindingActivityView<ShareViewModel>
	{
		private Button _buttonSave;
		private ListView _list;

		protected override void OnStart()
		{
			base.OnStart();

			ViewModel.LoadContacts();
		}

		protected override void OnViewModelSet()
		{
			RequestWindowFeature(WindowFeatures.ActionBar);

			SetContentView(Resource.Layout.Page_ShareView);

			_buttonSave = FindViewById<Button>(Resource.Id.buttonSave);
			_list = FindViewById<ListView>(Resource.Id.mvxList);
			_list.FastScrollEnabled = true;

			_buttonSave.Click += (s, e) =>
				{
					//Get emails
					var to = ViewModel.GetEmailTo();
					var subject = ViewModel.GetEmailSubject();
					var body = ViewModel.GetEmailBody();

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
						this.ShowInformation("Email Setup", "Please setup your Email on the device before sharing your Wish List!", "OK");
					}
				};
		}

		protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
		{
			base.OnActivityResult(requestCode, resultCode, data);

			if (requestCode == 101 && resultCode == Result.Ok)
				ViewModel.Finished();
		}
	}
}