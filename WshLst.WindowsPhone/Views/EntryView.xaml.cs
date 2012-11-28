using System;
using System.Windows;
using System.Windows.Navigation;
using Cirrious.MvvmCross.WindowsPhone.Views;
using WshLst.Core.ViewModels;

namespace WshLst.Views
{
	public class BaseEntryView : MvxPhonePage<EntryViewModel>
	{
	}

	public partial class EntryView : BaseEntryView
	{
		public EntryView()
		{
			InitializeComponent();
		}

		private void edit_Click(object sender, EventArgs e)
		{
			ViewModel.Edit();
		}

		private void delete_Click(object sender, EventArgs e)
		{
			if (MessageBox.Show("Are you sure you want to delete this item?", "Delete?", MessageBoxButton.OKCancel) ==
			    MessageBoxResult.OK)
				ViewModel.Delete();
		}

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);

			ViewModel.LoadEntry();
		}
	}
}