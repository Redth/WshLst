using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Cirrious.MvvmCross.WindowsPhone.Views;
using WshLst.Core.Models;
using WshLst.Core.ViewModels;

namespace WshLst.Views
{
	public class BaseLoginView : MvxPhonePage<LoginViewModel>
	{
	}

	public partial class LoginView : BaseLoginView
	{
		public LoginView()
		{
			InitializeComponent();
		}

		private void loginButton_Click(object sender, RoutedEventArgs e)
		{
			var button = sender as Button;
			if (button == null) return;
			var item = button.Tag as LoginPlatform;

			ViewModel.Login(item);
		}

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);

			if (Core.App.IsLaunch)
			{
				Core.App.IsLaunch = false;

				ViewModel.CheckLogin();
			}
		}
	}
}