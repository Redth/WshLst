using System.Collections;
using System.Collections.Generic;

namespace WshLst.Core.Models
{
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
	
	public class SelectableContact
	{
		public bool IsSelected { get; set; }
		public string DisplayName { get; set; }
		public string Email { get; set; }
	}
}