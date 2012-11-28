using System;
using System.Windows.Input;
using System.Collections.Generic;
using Cirrious.MvvmCross.ViewModels;
using Cirrious.MvvmCross.Commands;
using WshLst.Core.Models;

namespace WshLst.Core.ViewModels
{
	public class WishListViewModel : BaseViewModel
	{
		public WishListViewModel(string listId)
		{
			this.ListId = listId;
		}

		public string ListId = string.Empty;

		WishList wishList;
		public WishList WishList
		{
			get { return wishList; }
			set { wishList = value; RaisePropertyChanged("WishList"); }
		}

		List<Entry> entries;
		public List<Entry> Entries
		{
			get { return entries; }
			set { entries = value; RaisePropertyChanged("Entries"); }
		}

		public void Add()
		{	
			RequestNavigate<EditEntryViewModel>(new { listId = this.WishList.Id, entryId = string.Empty });
		}

		public void Select(Models.Entry entry)
		{
			RequestNavigate<EntryViewModel>(new { listId = this.WishList.Id, entryId = entry.Id });
		}

		public void Edit(Models.Entry entry)
		{
			RequestNavigate<EditEntryViewModel>(new { listId = this.WishList.Id, entryId = entry.Id });
		}

		public void Edit()
		{
			RequestNavigate<EditWishListViewModel>(new { listId = this.WishList.Id });
		}

		public void Delete(Entry entry)
		{
			this.IsLoading = true;

			App.Azure.GetTable<Entry>().DeleteAsync(entry).ContinueWith((t) =>
			{
				var ex = t.Exception;

				this.IsLoading = false;

				if (t.Status == System.Threading.Tasks.TaskStatus.RanToCompletion)
				{
					entries.Remove(entry);
					RaisePropertyChanged("Entries");
				}
			});
		}

		public void LoadListAndItems()
		{
			this.IsLoading = true;

			App.Azure.GetTable<WishList>().LookupAsync(this.ListId).ContinueWith((t) =>
			{
				var ex = t.Exception;

				if (t.Status == System.Threading.Tasks.TaskStatus.RanToCompletion)
				{
					this.WishList = t.Result;

					LoadItems();
				}
				else
				{
					this.IsLoading = false;
				}
			});
		}

		public void LoadItems()
		{
			this.IsLoading = true;

			App.Azure.GetTable<Entry>().Where(e => e.ListId == WishList.Id).ToListAsync().ContinueWith((t) => 
			{
				var ex = t.Exception;

				this.IsLoading = false;

				if (t.Status == System.Threading.Tasks.TaskStatus.RanToCompletion)
				{
					this.entries = new List<Models.Entry>();
					this.entries.AddRange(t.Result);
					RaisePropertyChanged("Entries");
				}
				else
				{
					this.ReportError("Failed to load items from WishList!");
				}

			});
		}


		public void Share()
		{
			RequestNavigate<ShareViewModel>(new { listId = this.ListId, listGuid = this.WishList.Guid });
		}
	}
}