using System;
using System.IO;
using System.IO.IsolatedStorage;
using Cirrious.MvvmCross.ViewModels;
using Newtonsoft.Json;
using WshLst.Core.Interfaces;

namespace WshLst.Core
{
	public class Settings : MvxApplicationObject, ISettingsProvider
	{
		public Settings()
		{
			UserId = string.Empty;
			AuthenticationProvider = -1;
		}

		public string UserId { get; set; }
		public int AuthenticationProvider { get; set; }
		
		public void Load()
		{
			try
			{
				var json = string.Empty;

				using (var isolatedStorage = IsolatedStorageFile.GetUserStoreForApplication())
				using (var file = new IsolatedStorageFileStream("Settings.json", FileMode.Open, isolatedStorage))
				using (var sw = new StreamReader(file))
				{
					json = sw.ReadToEnd();
				}

				var settings = JsonConvert.DeserializeObject<Settings>(json);

				UserId = settings.UserId;
				AuthenticationProvider = settings.AuthenticationProvider;
			}
			catch (Exception)
			{
			}
		}

		public void Save()
		{
			try
			{
				var json = JsonConvert.SerializeObject(this);

				using (var isolatedStorage = IsolatedStorageFile.GetUserStoreForApplication())
				using (var file = new IsolatedStorageFileStream("Settings.json", FileMode.Create, isolatedStorage))
				using (var sw = new StreamWriter(file))
				{
					sw.Write(json);
					sw.Flush();
				}
			}
			catch (Exception)
			{
			}
		}
	}
}