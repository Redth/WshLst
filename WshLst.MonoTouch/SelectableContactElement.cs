using System;
using System.Collections.Generic;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.CoreFoundation;
using MonoTouch.UIKit;
using CrossUI.Touch.Dialog.Elements;
using WshLst.Core.ViewModels;

namespace WshLst.MonoTouch
{
	public class SelectableContactElement : Element
	{
		public SelectableContactElement(ShareViewModel.SelectableContact contact) : base(contact.DisplayName)
		{
			this.Contact = contact;
		}

		public ShareViewModel.SelectableContact Contact { get; set; }

		protected override UITableViewCell GetCellImpl(UITableView tv)
		{
			var cell = tv.DequeueReusableCell("CheckboxDetailElement");

			if (cell == null)
				cell = new UITableViewCell(UITableViewCellStyle.Subtitle, "CheckboxDetailElement");

			cell.Accessory = this.Contact.IsSelected ? UITableViewCellAccessory.Checkmark : UITableViewCellAccessory.None;
			cell.TextLabel.Text = this.Contact.DisplayName;
			cell.DetailTextLabel.Text = this.Contact.Email;

			return cell;
		}

		public override void Selected(CrossUI.Touch.Dialog.DialogViewController dvc, UITableView tableView, NSIndexPath path)
		{
			this.Contact.IsSelected = !this.Contact.IsSelected;

			tableView.InvokeOnMainThread(() => tableView.ReloadRows(new NSIndexPath[] { path }, UITableViewRowAnimation.None));

			base.Selected(dvc, tableView, path);
		}
	}
}

