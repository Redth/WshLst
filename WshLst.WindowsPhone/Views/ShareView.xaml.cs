using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Tasks;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using WshLst.Core.ViewModels;
using Cirrious.MvvmCross.WindowsPhone.Views;

namespace WshLst.Views
{
	public class BaseShareView : MvxPhonePage<ShareViewModel> { }

	public partial class ShareView : BaseShareView
	{
		static EmailComposeTask email = new EmailComposeTask();

		public ShareView()
		{
			InitializeComponent();
		}

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);
			this.ViewModel.LoadContacts();
		}

		private void cancel_Click(object sender, EventArgs e)
		{
			this.ViewModel.Cancel();
		}

		private void send_Click(object sender, EventArgs e)
		{
			var to = this.ViewModel.GetEmailTo();
			var body = this.ViewModel.GetEmailBody();
			var subject = this.ViewModel.GetEmailSubject();

			email = new EmailComposeTask();
			email.To = to;
			email.Body = body;
			email.Subject = subject;
			email.Show();

			this.ViewModel.Finished();
		}
	}
}