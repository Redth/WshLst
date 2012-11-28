using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Cirrious.MvvmCross.Binding.Droid;
using Cirrious.MvvmCross.Binding.Droid.Views;
using WshLst.Core.ViewModels;
using WshLst.Core.Models;

namespace WshLst.MonoForAndroid.Views
{
	[Activity(Label = "Wish Lists", Icon = "@drawable/icontransparent")]
	public class WishListsView : MvxBindingActivityView<WishListsViewModel>
	{
		ListView list;

		protected override void OnStart()
		{
			this.ViewModel.LoadLists();

			base.OnStart();
		}
		protected override void OnViewModelSet()
		{
			RequestWindowFeature(WindowFeatures.ActionBar);

			SetContentView(Resource.Layout.Page_WishListsView);

			list = this.FindViewById<ListView>(Resource.Id.mvxList);

			list.ItemClick += (s, e) =>
			{
				var item = (MvxJavaContainer)list.Adapter.GetItem(e.Position);

				this.ViewModel.Select((Core.Models.WishList)item.Object);
			};

			this.RegisterForContextMenu(list);
		}

		public override void OnCreateContextMenu(IContextMenu menu, View v, IContextMenuContextMenuInfo menuInfo)
		{
			base.OnCreateContextMenu(menu, v, menuInfo);
			menu.Add("Delete Wish List");
			menu.Add("Edit Wish List");
		}

		public override bool OnContextItemSelected(IMenuItem menuItem)
		{
			var cmi = (AdapterView.AdapterContextMenuInfo)menuItem.MenuInfo;
			
			var item = (Cirrious.MvvmCross.Binding.Droid.MvxJavaContainer)this.list.Adapter.GetItem(cmi.Position);

			if (menuItem.TitleFormatted.ToString().Equals("Delete Wish List"))
			{
				this.ShowQuestion("Delete?", "Are you sure you want to delete this Wish List and all the items on it?", "Yes", "No",
					() =>
					{
						this.ViewModel.Delete((WishList)item.Object);
						this.ViewModel.LoadLists();
					}, null);
			}
			else
			{
				this.ViewModel.Edit((WishList)item.Object);
			}
			
			return true;
		}

		
		public override bool OnCreateOptionsMenu(IMenu menu)
		{
			MenuInflater.Inflate(Resource.Menu.ListsView_Menu, menu);
			return true;
		}

		public override bool OnOptionsItemSelected(IMenuItem item)
		{
			switch (item.ItemId)
			{
				case Resource.Id.addList:
					this.ViewModel.Add();
					return true;
				case Resource.Id.logout:
					this.ViewModel.Logout();
					return true;
			}
			return base.OnOptionsItemSelected(item);
		}

		public override void OnBackPressed()
		{
			//base.OnBackPressed();
		}

				
	}
}