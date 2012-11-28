using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Cirrious.MvvmCross.WindowsPhone.Views;
using WshLst.Core.Models;
using WshLst.Core.ViewModels;

namespace WshLst.Views
{
	public class BaseWishListsView : MvxPhonePage<WishListsViewModel> { }

	public partial class WishListsView : BaseWishListsView
	{
		public WishListsView()
		{
			InitializeComponent();
		}

		private void add_Click(object sender, EventArgs e)
		{
			this.ViewModel.Add();
		}

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);

			this.ViewModel.LoadLists();

			this.NavigationService.RemoveBackEntry();
		}

		private void itemSelected(object sender, SelectionChangedEventArgs e)
		{
			var lb = (sender as ListBox);

			if (lb.SelectedValue != null && lb.SelectedIndex >= 0)
			{
				var list = lb.SelectedValue as WishList;

				this.ViewModel.Select(list);

				lb.SelectedIndex = -1;
			}
		}

		private void deleteWishList_Click(object sender, RoutedEventArgs e)
		{
			var wishList = (sender as MenuItem).Tag as WishList;

			if (MessageBox.Show("Are you sure you want to delete the Wish List " + wishList.Name + " and all of its items?", "Delete Wish List?", MessageBoxButton.OKCancel)
				== MessageBoxResult.OK)
				this.ViewModel.Delete(wishList);
		}

		private void logout_Click(object sender, EventArgs e)
		{
			this.ViewModel.Logout();
		}

		private void editWishList_Click(object sender, RoutedEventArgs e)
		{
			var wishList = (sender as MenuItem).Tag as WishList;
			
			this.ViewModel.Edit(wishList);
		}
	}
}