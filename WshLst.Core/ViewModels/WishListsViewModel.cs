using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Cirrious.MvvmCross.Commands;
using Cirrious.MvvmCross.ExtensionMethods;
using WshLst.Core.Interfaces;
using WshLst.Core.Models;

namespace WshLst.Core.ViewModels
{
	public class WishListsViewModel : BaseViewModel
	{
		private List<WishList> _lists;

		public WishListsViewModel()
		{
			LoadLists();
		}

		public List<WishList> Lists
		{
			get { return _lists; }
			set
			{
				_lists = value;
				RaisePropertyChanged(() => Lists);
			}
		}

		public ICommand AddCommand
		{
			get { return new MvxRelayCommand(Add); }
		}

		public void Add()
		{
			RequestNavigate<EditWishListViewModel>();
		}

		public ICommand SelectCommand
		{
			get { return new MvxRelayCommand<WishList>(Select); }
		}

		public void Select(WishList item)
		{
			RequestNavigate<WishListViewModel>(new {listId = item.Id});
		}

		public ICommand EditCommand
		{
			get { return new MvxRelayCommand<WishList>(Edit); }
		}
		
		public void Edit(WishList item)
		{
			RequestNavigate<EditWishListViewModel>(new {listId = item.Id});
		}

		public ICommand DeleteCommand
		{
			get { return new MvxRelayCommand<WishList>(Delete); }
		}

		public void Delete(WishList item)
		{
			IsLoading = true;

			App.Azure.GetTable<WishList>().DeleteAsync(item).ContinueWith(t =>
				{
					var ex = t.Exception;

					IsLoading = false;

					if (t.Status != TaskStatus.RanToCompletion)
					{
						ReportError("Failed to delete WishList!");
					}
					else
					{
						_lists.Remove(item);
						RaisePropertyChanged(() => Lists);
					}
				});
		}

		public void LoadLists()
		{
			IsLoading = true;

			var settings = this.GetService<ISettingsProvider>();
			var userId = settings.UserId;

			App.Azure.GetTable<WishList>().Where(wl => wl.UserId == userId).ToListAsync().ContinueWith(t =>
				{
					var ex = t.Exception;

					if (t.Status == TaskStatus.RanToCompletion)
					{
						_lists = new List<WishList>();
						_lists.AddRange(t.Result);
						RaisePropertyChanged(() => Lists);
					}
					else
						_lists = new List<WishList>();

					IsLoading = false;
				});
		}

		public void Logout()
		{
			var settings = this.GetService<ISettingsProvider>();

			settings.UserId = string.Empty;
			settings.AuthenticationProvider = -1;
			settings.Save();

			RequestNavigate<LoginViewModel>();
		}
	}
}