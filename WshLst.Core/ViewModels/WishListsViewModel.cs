using System;
using System.Windows.Input;
using System.Collections.Generic;
using Cirrious.MvvmCross.ViewModels;
using Cirrious.MvvmCross.Commands;
using Cirrious.MvvmCross.ExtensionMethods;
using WshLst.Core.Models;

namespace WshLst.Core.ViewModels
{
	public class WishListsViewModel : BaseViewModel
	{
		public WishListsViewModel()
		{
			LoadLists();
		}

		List<Models.WishList> lists;
		public List<Models.WishList> Lists
		{
			get { return lists; }
			set { lists = value; RaisePropertyChanged("Lists"); }
		}
		
		public void Add()
		{
			RequestNavigate<EditWishListViewModel>();
		}

		public void Select(Models.WishList item)
		{
			RequestNavigate<WishListViewModel>(new { listId = item.Id });
		}

		public void Edit(WishList item)
		{
			RequestNavigate<EditWishListViewModel>(new { listId = item.Id });
		}

		public void Delete(Models.WishList item)
		{
			this.IsLoading = true;

			App.Azure.GetTable<Models.WishList>().DeleteAsync(item).ContinueWith((t) =>
			{
				var ex = t.Exception;

				this.IsLoading = false;

				if (t.Status != System.Threading.Tasks.TaskStatus.RanToCompletion)
				{
					ReportError("Failed to delete WishList!");
				}
				else
				{
					this.lists.Remove(item);
					this.RaisePropertyChanged("Lists");
				}
			});
		}

		public void LoadLists()
		{
			IsLoading = true;

			var settings = this.GetService<Interfaces.ISettingsProvider>();
			var userId = settings.UserId;

			App.Azure.GetTable<Models.WishList>().Where(wl => wl.UserId == userId).ToListAsync().ContinueWith((t) =>
			{
				var ex = t.Exception;

				if (t.Status == System.Threading.Tasks.TaskStatus.RanToCompletion)
				{
					lists = new List<WishList>();
					lists.AddRange(t.Result);
					RaisePropertyChanged("Lists");
				}
				else
					lists = new List<Models.WishList>();

				IsLoading = false;				
			});
		}

		public void Logout()
		{
			var settings = this.GetService<Interfaces.ISettingsProvider>();

			settings.UserId = string.Empty;
			settings.AuthenticationProvider = -1;
			settings.Save();

			RequestNavigate<ViewModels.LoginViewModel>();
		}
	}
}