using System.Collections.Generic;
using System.Threading.Tasks;
using WshLst.Core.Models;

namespace WshLst.Core.ViewModels
{
	public class WishListViewModel : BaseViewModel
	{
		public string ListId = string.Empty;
		private List<Entry> _entries;

		private WishList _wishList;

		public WishListViewModel(string listId)
		{
			ListId = listId;
		}

		public WishList WishList
		{
			get { return _wishList; }
			set
			{
				_wishList = value;
				RaisePropertyChanged("WishList");
			}
		}

		public List<Entry> Entries
		{
			get { return _entries; }
			set
			{
				_entries = value;
				RaisePropertyChanged("Entries");
			}
		}

		public void Add()
		{
			RequestNavigate<EditEntryViewModel>(new {listId = WishList.Id, entryId = string.Empty});
		}

		public void Select(Entry entry)
		{
			RequestNavigate<EntryViewModel>(new {listId = WishList.Id, entryId = entry.Id});
		}

		public void Edit(Entry entry)
		{
			RequestNavigate<EditEntryViewModel>(new {listId = WishList.Id, entryId = entry.Id});
		}

		public void Edit()
		{
			RequestNavigate<EditWishListViewModel>(new {listId = WishList.Id});
		}

		public void Delete(Entry entry)
		{
			IsLoading = true;

			App.Azure.GetTable<Entry>().DeleteAsync(entry).ContinueWith(t =>
				{
					var ex = t.Exception;

					IsLoading = false;

					if (t.Status == TaskStatus.RanToCompletion)
					{
						_entries.Remove(entry);
						RaisePropertyChanged("Entries");
					}
				});
		}

		public void LoadListAndItems()
		{
			IsLoading = true;

			App.Azure.GetTable<WishList>().LookupAsync(ListId).ContinueWith(t =>
				{
					var ex = t.Exception;

					if (t.Status == TaskStatus.RanToCompletion)
					{
						WishList = t.Result;

						LoadItems();
					}
					else
					{
						IsLoading = false;
					}
				});
		}

		public void LoadItems()
		{
			IsLoading = true;

			App.Azure.GetTable<Entry>().Where(e => e.ListId == WishList.Id).ToListAsync().ContinueWith(t =>
				{
					var ex = t.Exception;

					IsLoading = false;

					if (t.Status == TaskStatus.RanToCompletion)
					{
						_entries = new List<Entry>();
						_entries.AddRange(t.Result);
						RaisePropertyChanged("Entries");
					}
					else
					{
						ReportError("Failed to load items from WishList!");
					}
				});
		}


		public void Share()
		{
			RequestNavigate<ShareViewModel>(new {listId = ListId, listGuid = WishList.Guid});
		}
	}
}