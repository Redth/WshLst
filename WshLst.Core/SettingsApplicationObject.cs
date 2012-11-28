using System;
using System.IO;
using System.IO.IsolatedStorage;
using Cirrious.MvvmCross.Application;
using Cirrious.MvvmCross.Core;
using Cirrious.MvvmCross.Interfaces;
using Cirrious.MvvmCross.ViewModels;
using WshLst.Core.Interfaces;
using Microsoft.WindowsAzure.MobileServices;


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

				var settings = Newtonsoft.Json.JsonConvert.DeserializeObject<Settings>(json);

				this.UserId = settings.UserId;
				this.AuthenticationProvider = settings.AuthenticationProvider;
			}
			catch {  }
		}

		public void Save()
		{
			try
			{
				var json = Newtonsoft.Json.JsonConvert.SerializeObject(this);

				using (var isolatedStorage = IsolatedStorageFile.GetUserStoreForApplication())
				using (var file = new IsolatedStorageFileStream("Settings.json", FileMode.Create, isolatedStorage))
				using (var sw = new StreamWriter(file))
				{
					sw.Write(json);
					sw.Flush();
				}
			}
			catch { }
		}
	}
}
