using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Cirrious.MvvmCross.WindowsPhone.Views;
using Microsoft.Phone.Controls;
using WshLst.Core.Models;
using WshLst.Core.ViewModels;

namespace WshLst.Views
{
	public class BaseWishListView : MvxPhonePage<WishListViewModel>
	{
	}

	public partial class WishListView : BaseWishListView
	{
		public WishListView()
		{
			InitializeComponent();
		}

		private void ItemSelected(object sender, SelectionChangedEventArgs e)
		{
			var lb = sender as ListBox;

			if (lb == null || lb.SelectedIndex < 0)
				return;

			var item = lb.SelectedValue as Entry;

			if (item == null)
				return;

			ViewModel.Select(item);
			
			lb.SelectedIndex = -1;
		}

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);

			ViewModel.LoadListAndItems();
		}

		private void add_Click(object sender, EventArgs e)
		{
			ViewModel.Add();
		}

		private void deleteItem_Click(object sender, RoutedEventArgs e)
		{
			var item = ((MenuItem) sender).Tag as Entry;

			if (item == null)
				return;

			if (MessageBox.Show("Are you sure you want to delete: " + item.Name + "?", "Delete?", MessageBoxButton.OKCancel) ==
			    MessageBoxResult.OK)
				ViewModel.Delete(item);
		}

		private void share_Click(object sender, EventArgs e)
		{
			ViewModel.Share();
		}
	}
}