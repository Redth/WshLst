using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Cirrious.MvvmCross.Commands;
using WshLst.Core.Models;

namespace WshLst.Core.ViewModels
{
	public class WishListViewModel : BaseViewModel
	{
		public string ListId = string.Empty;
		private ObservableCollection<Entry> _entries;

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

		public ObservableCollection<Entry> Entries
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

		public ICommand SelectCommand
		{
			get { return new MvxRelayCommand<Entry>(Select); }
		}

		public void Select(Entry entry)
		{
			RequestNavigate<EntryViewModel>(new {listId = WishList.Id, entryId = entry.Id});
		}

		public ICommand EditCommand
		{
			get { return new MvxRelayCommand<Entry>(Edit); }
		}

		public void Edit(Entry entry)
		{
			RequestNavigate<EditEntryViewModel>(new {listId = WishList.Id, entryId = entry.Id});
		}

		public ICommand EditWishListCommand
		{
			get { return new MvxRelayCommand(EditWishList); }
		}

		public void EditWishList()
		{
			RequestNavigate<EditWishListViewModel>(new {listId = WishList.Id});
		}

		public ICommand DeleteCommand
		{
			get { return new MvxRelayCommand<Entry>(Delete); }
		}

		public void Delete(Entry entry)
		{
			IsLoading = true;

			var entryId = entry.Id;

			App.Azure.GetTable<Entry>().DeleteAsync(entry).ContinueWith(t =>
				{
					var ex = t.Exception;

					IsLoading = false;

					this.InvokeOnMainThread(() =>
						{
							if (t.Status == TaskStatus.RanToCompletion)
							{
								if (_entries == null)
									return;

								_entries.Remove(entry);
								RaisePropertyChanged("Entries");
							}
						});
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

					InvokeOnMainThread(() =>
						{
							if (t.Status == TaskStatus.RanToCompletion)
							{
								if (_entries == null)
									_entries = new ObservableCollection<Entry>();

								_entries.Clear();

								foreach (var e in t.Result)	
									_entries.Add(e);

								RaisePropertyChanged("Entries");
							}
							else
							{
								ReportError("Failed to load items from WishList!");
							}
						});
				});
		}

		public ICommand ShareCommand
		{
			get { return new MvxRelayCommand(Share); }
		}

		public void Share()
		{
			RequestNavigate<ShareViewModel>(new {listId = ListId, listGuid = WishList.Guid});
		}
	}
}