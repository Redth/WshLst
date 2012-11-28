using System;
using System.Windows.Navigation;
using Cirrious.MvvmCross.WindowsPhone.Views;
using Microsoft.Phone.Tasks;
using WshLst.Core.ViewModels;

namespace WshLst.Views
{
	public class BaseShareView : MvxPhonePage<ShareViewModel>
	{
	}

	public partial class ShareView : BaseShareView
	{
		private static EmailComposeTask email = new EmailComposeTask();

		public ShareView()
		{
			InitializeComponent();
		}

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);
			ViewModel.LoadContacts();
		}

		private void cancel_Click(object sender, EventArgs e)
		{
			ViewModel.Cancel();
		}

		private void send_Click(object sender, EventArgs e)
		{
			var to = ViewModel.GetEmailTo();
			var body = ViewModel.GetEmailBody();
			var subject = ViewModel.GetEmailSubject();

			email = new EmailComposeTask {To = to, Body = body, Subject = subject};
			email.Show();

			ViewModel.Finished();
		}
	}
}