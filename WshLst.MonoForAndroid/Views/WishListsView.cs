using Android.App;
using Android.Views;
using Android.Widget;
using Cirrious.MvvmCross.Binding.Droid;
using Cirrious.MvvmCross.Binding.Droid.Views;
using WshLst.Core.Models;
using WshLst.Core.ViewModels;

namespace WshLst.MonoForAndroid.Views
{
	[Activity(Label = "Wish Lists", Icon = "@drawable/icontransparent")]
	public class WishListsView : MvxBindingActivityView<WishListsViewModel>
	{
		private ListView _list;

		protected override void OnStart()
		{
			ViewModel.LoadLists();

			base.OnStart();
		}

		protected override void OnViewModelSet()
		{
			RequestWindowFeature(WindowFeatures.ActionBar);

			SetContentView(Resource.Layout.Page_WishListsView);

			_list = FindViewById<ListView>(Resource.Id.mvxList);
			
			RegisterForContextMenu(_list);
		}

		public override void OnCreateContextMenu(IContextMenu menu, View v, IContextMenuContextMenuInfo menuInfo)
		{
			base.OnCreateContextMenu(menu, v, menuInfo);
			menu.Add("Delete Wish List");
			menu.Add("Edit Wish List");
		}

		public override bool OnContextItemSelected(IMenuItem menuItem)
		{
			var cmi = (AdapterView.AdapterContextMenuInfo) menuItem.MenuInfo;

			var item = (MvxJavaContainer) _list.Adapter.GetItem(cmi.Position);

			if (menuItem.TitleFormatted.ToString().Equals("Delete Wish List"))
			{
				this.ShowQuestion("Delete?", "Are you sure you want to delete this Wish List and all the items on it?", "Yes", "No",
				                  () =>
					                  {
						                  ViewModel.Delete((WishList) item.Object);
						                  ViewModel.LoadLists();
					                  }, null);
			}
			else
			{
				ViewModel.Edit((WishList) item.Object);
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
					ViewModel.Add();
					return true;
				case Resource.Id.logout:
					ViewModel.Logout();
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