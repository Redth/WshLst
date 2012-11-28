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
	public class BaseEditWishListView : MvxPhonePage<EditWishListViewModel> { }

	public partial class EditWishListView : BaseEditWishListView
	{
		public EditWishListView()
		{
			InitializeComponent();
		}

		private void cancel_Click(object sender, EventArgs e)
		{
			this.ViewModel.Cancel();
		}

		private void save_Click(object sender, EventArgs e)
		{
			this.ViewModel.Save();
		}

		private void text_Changed(object sender, TextChangedEventArgs e)
		{
			((TextBox)sender).GetBindingExpression(TextBox.TextProperty).UpdateSource();
		}

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);

			this.ViewModel.LoadList();
		}
	}
}