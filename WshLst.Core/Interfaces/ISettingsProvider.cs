using System;

namespace WshLst.Core.Interfaces
{
	public interface ISettingsProvider
	{
		string UserId { get; set; }
		int AuthenticationProvider { get; set; }
		
		void Load();
		void Save();
	}
}