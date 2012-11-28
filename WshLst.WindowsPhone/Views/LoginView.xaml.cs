using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using WshLst.Core.ViewModels;
using Cirrious.MvvmCross.WindowsPhone.Views;


namespace WshLst.Views
{
	public class BaseLoginView : MvxPhonePage<LoginViewModel> { }

	public partial class LoginView : BaseLoginView
	{
		public LoginView()
		{
			InitializeComponent();
		}
				
		private void loginButton_Click(object sender, RoutedEventArgs e)
		{
			var button = sender as Button;
			var item = button.Tag as WshLst.Core.Models.LoginPlatform;

			this.ViewModel.Login(item);
		}

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);

			if (WshLst.Core.App.IsLaunch)
			{
				WshLst.Core.App.IsLaunch = false;

				this.ViewModel.CheckLogin();
			}
		}
	}
}