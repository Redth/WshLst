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
	[Activity(Label = "Loading Wish List...", Icon = "@drawable/icontransparent")]
	public class ListViewList : MvxBindingActivityView<WishListViewModel>
	{
		ListView list;

		protected override void OnStart()
		{
			base.OnStart();

			this.ViewModel.LoadListAndItems();
		}

		protected override void OnViewModelSet()
		{
			RequestWindowFeature(WindowFeatures.ActionBar);

			SetContentView(Resource.Layout.Page_WishListView);

			list = this.FindViewById<ListView>(Resource.Id.mvxList);

			list.ItemClick += (s, e) =>
			{
				var item = (MvxJavaContainer)list.Adapter.GetItem(e.Position);

				this.ViewModel.Select((Core.Models.Entry)item.Object);
			};

			this.RegisterForContextMenu(list);

			this.ViewModel.PropertyChanged += (s, e) =>
			{
				if (e.PropertyName.Equals("WishList"))
				{
					this.Title = this.ViewModel.WishList.Name;
				}
			};

			this.ViewModel.LoadListAndItems();
		}
		
		public override void OnCreateContextMenu(IContextMenu menu, View v, IContextMenuContextMenuInfo menuInfo)
		{
			base.OnCreateContextMenu(menu, v, menuInfo);
			menu.Add("Edit Item");
			menu.Add("Delete Item");
		}

		public override bool OnContextItemSelected(IMenuItem menuItem)
		{
			if (menuItem.TitleFormatted.ToString().Equals("Edit Item"))
			{
				var cmi = (AdapterView.AdapterContextMenuInfo)menuItem.MenuInfo;

				var item = (Cirrious.MvvmCross.Binding.Droid.MvxJavaContainer)this.list.Adapter.GetItem(cmi.Position);

				this.ViewModel.Edit((Core.Models.Entry)item.Object);
			}
			else
			{
				var cmi = (AdapterView.AdapterContextMenuInfo)menuItem.MenuInfo;

				var item = (Cirrious.MvvmCross.Binding.Droid.MvxJavaContainer)this.list.Adapter.GetItem(cmi.Position);

				this.ShowQuestion("Delete?", "Are you sure you want to delete this item?", "Yes", "No",
					() =>
					{
						this.ViewModel.Delete((Core.Models.Entry)item.Object);
						this.ViewModel.LoadItems();
					}, null);
			}

			return true;
		}


		public override bool OnCreateOptionsMenu(IMenu menu)
		{
			MenuInflater.Inflate(Resource.Menu.ListView_Menu, menu);
			return true;
		}

		public override bool OnOptionsItemSelected(IMenuItem item)
		{
			switch (item.ItemId)
			{
				case Resource.Id.addEntry:
					this.ViewModel.Add();
					return true;
				case Resource.Id.shareWishList:
					this.ViewModel.Share();
					return true;
			}
			return base.OnOptionsItemSelected(item);
		}
	}
}