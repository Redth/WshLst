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
	public class BaseWishListsView : MvxPhonePage<WishListsViewModel>
	{
	}

	public partial class WishListsView : BaseWishListsView
	{
		public WishListsView()
		{
			InitializeComponent();
		}

		private void add_Click(object sender, EventArgs e)
		{
			ViewModel.Add();
		}

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);

			ViewModel.LoadLists();

			NavigationService.RemoveBackEntry();
		}

		private void ItemSelected(object sender, SelectionChangedEventArgs e)
		{
			var lb = (sender as ListBox);

			if (lb == null || lb.SelectedValue == null || lb.SelectedIndex < 0) 
				return;

			var list = lb.SelectedValue as WishList;

			ViewModel.Select(list);

			lb.SelectedIndex = -1;
		}

		private void deleteWishList_Click(object sender, RoutedEventArgs e)
		{
			var wishList = ((MenuItem) sender).Tag as WishList;

			if (wishList == null)
				return;

			if (MessageBox.Show("Are you sure you want to delete the Wish List " + wishList.Name + " and all of its items?",
			                    "Delete Wish List?", MessageBoxButton.OKCancel)
			    == MessageBoxResult.OK)
				ViewModel.Delete(wishList);
		}

		private void logout_Click(object sender, EventArgs e)
		{
			ViewModel.Logout();
		}

		private void editWishList_Click(object sender, RoutedEventArgs e)
		{
			var wishList = ((MenuItem) sender).Tag as WishList;

			if (wishList == null)
				return;

			ViewModel.Edit(wishList);
		}
	}
}