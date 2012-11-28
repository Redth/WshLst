using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using Xamarin.Contacts;
using Cirrious.MvvmCross.ExtensionMethods;

namespace WshLst.Core.ViewModels
{
	public class ShareViewModel : BaseViewModel
	{
		public ShareViewModel(string listId, string listGuid)
		{
			this.ListId = listId;
			this.ListGuid = listGuid;
		}

		public string ListId { get; set; }
		public string ListGuid { get; set; }

		List<SelectableContact> contacts;
		public List<SelectableContact> Contacts
		{
			get { return contacts; }
			set { contacts = value; RaisePropertyChanged("Contacts"); }
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

			contacts = (from c in allContacts
						orderby c.DisplayName
						where c.Emails != null && c.Emails.Count() > 0
						select new SelectableContact() { DisplayName = c.DisplayName, Email = (c.Emails != null && c.Emails.Count() > 0 ? c.Emails.FirstOrDefault().Address : string.Empty), IsSelected = false }).ToList();

			RaisePropertyChanged("Contacts");
		}

		public void Cancel()
		{
			RequestClose(this);
		}

		public void Finished()
		{
			RequestNavigate<WishListViewModel>(new { listId = this.ListId });
		}

		public string GetEmailTo()
		{
			if (contacts == null || contacts.Count <= 0)
				return string.Empty;

			var selContacts = from c in contacts where c.IsSelected select c.Email;

			return string.Join("; ", selContacts);
		}

		public string GetEmailSubject()
		{
			return "I've shared a Wish List with you!";
		}


		public string GetEmailBody()
		{
			var url = string.Format("https://wshlst.azurewebsites.net/?id={0}", this.ListGuid);

			return "Hi, I'd like to share my wish list with you!  You can see my Wish List online at: \r\n\r\n" + url;
		}

		public class SelectableContactGroup : IEnumerable<SelectableContact>
		{
			public string Title { get; set; }
			public List<SelectableContact> Items { get; set; }

			public IEnumerator<SelectableContact> GetEnumerator()
			{
				return Items.GetEnumerator();
			}

			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
			{
				return Items.GetEnumerator();
			}

			IEnumerator<SelectableContact> IEnumerable<SelectableContact>.GetEnumerator()
			{
				return Items.GetEnumerator();
			}
		}

		public class SelectableContact
		{
			public bool IsSelected { get; set; }
			public string DisplayName { get; set; }
			public string Email { get; set; } 
		}
		
	}
}
