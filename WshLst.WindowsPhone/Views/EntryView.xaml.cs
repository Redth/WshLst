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
	public class BaseEntryView : MvxPhonePage<EntryViewModel> { }

	public partial class EntryView : BaseEntryView
	{
		public EntryView()
		{
			InitializeComponent();
		}

		private void edit_Click(object sender, EventArgs e)
		{
			this.ViewModel.Edit();
		}

		private void delete_Click(object sender, EventArgs e)
		{
			if (MessageBox.Show("Are you sure you want to delete this item?", "Delete?", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
				this.ViewModel.Delete();
		}

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);

			this.ViewModel.LoadEntry();
		}
	}
}