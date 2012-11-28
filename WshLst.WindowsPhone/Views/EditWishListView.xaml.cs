using System;
using System.Windows.Controls;
using System.Windows.Navigation;
using Cirrious.MvvmCross.WindowsPhone.Views;
using WshLst.Core.ViewModels;

namespace WshLst.Views
{
	public class BaseEditWishListView : MvxPhonePage<EditWishListViewModel>
	{
	}

	public partial class EditWishListView : BaseEditWishListView
	{
		public EditWishListView()
		{
			InitializeComponent();
		}

		private void cancel_Click(object sender, EventArgs e)
		{
			ViewModel.Cancel();
		}

		private void save_Click(object sender, EventArgs e)
		{
			ViewModel.Save();
		}

		private void text_Changed(object sender, TextChangedEventArgs e)
		{
			((TextBox) sender).GetBindingExpression(TextBox.TextProperty).UpdateSource();
		}

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);

			ViewModel.LoadList();
		}
	}
}