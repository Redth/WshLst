using System;
using System.Collections.Generic;
using System.Drawing;
using Cirrious.MvvmCross.Dialog.Touch;
using Cirrious.MvvmCross.Touch.Interfaces;
using Cirrious.MvvmCross.Views;
using CrossUI.Touch.Dialog.Elements;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using WshLst.Core.Models;
using WshLst.Core.ViewModels;

namespace WshLst.MonoTouch
{
	public class EditWishListView : BaseDialogViewController<EditWishListViewModel>
	{
		public EditWishListView(MvxShowViewModelRequest request) 
			: base (request, UITableViewStyle.Grouped, new RootElement(), true)
		{
		}
	
		UIBarButtonItem _buttonSave = new UIBarButtonItem(UIBarButtonSystemItem.Done);
		UIBarButtonItem _buttonCancel = new UIBarButtonItem(UIBarButtonSystemItem.Cancel);

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
		
			NavigationItem.RightBarButtonItem = _buttonSave;
			NavigationItem.LeftBarButtonItem = _buttonCancel;
		
			_buttonSave.Clicked += (sender, e) => this.ViewModel.Save();
			_buttonCancel.Clicked += (sender, e) => this.ViewModel.Cancel();
		
			this.Root = new RootElement("New Wish List")
			{
				new Section()
				{
					new EntryElement("Name", "Wish List Name").Bind(this, "{'Value':{'Path':'Name'}}"),
					new EntryElement("Description", "Description").Bind(this, "{'Value':{'Path':'Description'}}")
				}
			};
		}
	}
}

