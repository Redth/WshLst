using Android.App;
using Android.Views;
using Android.Widget;
using Cirrious.MvvmCross.Binding.Droid;
using Cirrious.MvvmCross.Binding.Droid.Views;
using WshLst.Core.Models;
using WshLst.Core.ViewModels;

namespace WshLst.MonoForAndroid.Views
{
	[Activity(Label = "Loading Wish List...", Icon = "@drawable/icontransparent")]
	public class ListViewList : MvxBindingActivityView<WishListViewModel>
	{
		private ListView _list;

		protected override void OnStart()
		{
			base.OnStart();

			ViewModel.LoadListAndItems();
		}

		protected override void OnViewModelSet()
		{
			RequestWindowFeature(WindowFeatures.ActionBar);

			SetContentView(Resource.Layout.Page_WishListView);

			_list = FindViewById<ListView>(Resource.Id.mvxList);

			_list.ItemClick += (s, e) =>
				{
					var item = (MvxJavaContainer) _list.Adapter.GetItem(e.Position);

					ViewModel.Select((Entry) item.Object);
				};

			RegisterForContextMenu(_list);

			ViewModel.PropertyChanged += (s, e) =>
				{
					if (e.PropertyName.Equals("WishList"))
					{
						Title = ViewModel.WishList.Name;
					}
				};

			ViewModel.LoadListAndItems();
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
				var cmi = (AdapterView.AdapterContextMenuInfo) menuItem.MenuInfo;

				var item = (MvxJavaContainer) _list.Adapter.GetItem(cmi.Position);

				ViewModel.Edit((Entry) item.Object);
			}
			else
			{
				var cmi = (AdapterView.AdapterContextMenuInfo) menuItem.MenuInfo;

				var item = (MvxJavaContainer) _list.Adapter.GetItem(cmi.Position);

				this.ShowQuestion("Delete?", "Are you sure you want to delete this item?", "Yes", "No",
				                  () =>
					                  {
						                  ViewModel.Delete((Entry) item.Object);
						                  ViewModel.LoadItems();
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
					ViewModel.Add();
					return true;
				case Resource.Id.shareWishList:
					ViewModel.Share();
					return true;
			}
			return base.OnOptionsItemSelected(item);
		}
	}
}