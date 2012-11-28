using System;
using System.Collections.Generic;
using System.Drawing;
using Cirrious.MvvmCross.Binding.Touch.ExtensionMethods;
using Cirrious.MvvmCross.Binding.Touch.Views;
using Cirrious.MvvmCross.Views;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using WshLst.Core.Models;
using WshLst.Core.ViewModels;

namespace WshLst.MonoTouch
{
	public class MvxDeleteBindableTableViewSource : MvxBindableTableViewSource
	{
		public MvxDeleteBindableTableViewSource(UITableView tableView, UITableViewCellStyle cellStyle, 
		                                         NSString cellIdentifier, string bindingText, UITableViewCellAccessory accessory) 
			: base(tableView, cellStyle, cellIdentifier, bindingText, accessory)
		{
		}

		public event Action<UITableView, UITableViewCellEditingStyle, NSIndexPath> OnShouldCommitEditingStyle;

		public override bool CanEditRow(UITableView tableView, NSIndexPath indexPath)
		{
			return true;
		}

		public override UITableViewCellEditingStyle EditingStyleForRow(UITableView tableView, NSIndexPath indexPath)
		{
			return UITableViewCellEditingStyle.Delete;
		}

		public override void CommitEditingStyle(UITableView tableView, UITableViewCellEditingStyle editingStyle, NSIndexPath indexPath)
		{
			if (this.OnShouldCommitEditingStyle != null)
				this.OnShouldCommitEditingStyle(tableView, editingStyle, indexPath);
		}
	}
}

