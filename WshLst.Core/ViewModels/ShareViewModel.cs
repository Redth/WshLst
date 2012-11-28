using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Cirrious.MvvmCross.Commands;
using Cirrious.MvvmCross.ExtensionMethods;
using Xamarin.Contacts;

namespace WshLst.Core.ViewModels
{
	public class ShareViewModel : BaseViewModel
	{
		private List<SelectableContact> _contacts;

		public ShareViewModel(string listId, string listGuid)
		{
			ListId = listId;
			ListGuid = listGuid;
		}

		public string ListId { get; set; }
		public string ListGuid { get; set; }

		public List<SelectableContact> Contacts
		{
			get { return _contacts; }
			set
			{
				_contacts = value;
				RaisePropertyChanged(() => Contacts);
			}
		}

		public void LoadContacts()
		{
#if MONOANDROID
			var androidGlobals = this.GetService<Cirrious.MvvmCross.Droid.Interfaces.IMvxAndroidGlobals>();
			var ab = new AddressBook(androidGlobals.ApplicationContext);
#else
			var ab = new AddressBook();
#endif

			var allContacts = ab.ToList();

			_contacts = (from c in allContacts
			             orderby c.DisplayName
			             where c.Emails != null && c.Emails.Any()
			             let email = c.Emails.FirstOrDefault()
			             where email != null && !string.IsNullOrEmpty(email.Address)
			             select
				             new SelectableContact
					             {
						             DisplayName = c.DisplayName,
						             Email = email.Address,
						             IsSelected = false
					             }).ToList();

			RaisePropertyChanged(() => Contacts);
		}

		public ICommand CancelCommand
		{
			get { return new MvxRelayCommand(Cancel); }
		}

		public void Cancel()
		{
			RequestClose(this);
		}
		
		public ICommand FinishedCommand
		{
			get { return new MvxRelayCommand(Finished); }
		}

		public void Finished()
		{
			RequestNavigate<WishListViewModel>(new {listId = ListId});
		}

		public string GetEmailTo()
		{
			if (_contacts == null || _contacts.Count <= 0)
				return string.Empty;

			var selContacts = from c in _contacts where c.IsSelected select c.Email;

			return string.Join("; ", selContacts);
		}

		public string GetEmailSubject()
		{
			return "I've shared a Wish List with you!";
		}

		public string GetEmailBody()
		{
			var url = string.Format(Config.AZURE_WEBSITE_URL + "/?id={0}", ListGuid);

			return "Hi, I'd like to share my wish list with you!  You can see my Wish List online at: \r\n\r\n" + url;
		}

		public class SelectableContact
		{
			public bool IsSelected { get; set; }
			public string DisplayName { get; set; }
			public string Email { get; set; }
		}

		public class SelectableContactGroup : IEnumerable<SelectableContact>
		{
			public string Title { get; set; }
			public List<SelectableContact> Items { get; set; }

			IEnumerator IEnumerable.GetEnumerator()
			{
				return Items.GetEnumerator();
			}

			IEnumerator<SelectableContact> IEnumerable<SelectableContact>.GetEnumerator()
			{
				return Items.GetEnumerator();
			}

			public IEnumerator<SelectableContact> GetEnumerator()
			{
				return Items.GetEnumerator();
			}
		}
	}
}