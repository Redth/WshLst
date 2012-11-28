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
	public class BaseWishListView : MvxPhonePage<WishListViewModel> { }

	public partial class WishListView : BaseWishListView
	{
		public WishListView()
		{
			InitializeComponent();
		}

		private void itemSelected(object sender, SelectionChangedEventArgs e)
		{
			var lb = sender as ListBox;

			if (lb.SelectedIndex >= 0)
			{
				var item = lb.SelectedValue as Core.Models.Entry;

				this.ViewModel.Select(item);
			}

			lb.SelectedIndex = -1;
		}

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);

			this.ViewModel.LoadListAndItems();
		}

		private void add_Click(object sender, EventArgs e)
		{
			this.ViewModel.Add();
		}

		private void deleteItem_Click(object sender, RoutedEventArgs e)
		{
			var item = (sender as MenuItem).Tag as Core.Models.Entry;

			if (MessageBox.Show("Are you sure you want to delete: " + item.Name + "?", "Delete?", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
				this.ViewModel.Delete(item);
		}

		private void share_Click(object sender, EventArgs e)
		{
			this.ViewModel.Share();
		}
	}
}