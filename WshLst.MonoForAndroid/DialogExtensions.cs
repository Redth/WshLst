using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace WshLst.MonoForAndroid
{
	public static class DialogExtensions
	{
		public static AlertDialog ShowQuestion(this Activity activity, string title, string message, string positiveButton, string negativeButton, Action positive, Action negative)
		{
			AlertDialog dialog = null;

			var builder = new AlertDialog.Builder(activity);
			builder.SetTitle(title);
			builder.SetMessage(message);
			builder.SetPositiveButton(positiveButton, (s, e) => 
			{
				dialog.Cancel();
				dialog.Dismiss();

				if (positive != null)
					positive(); 
			});

			builder.SetNegativeButton(negativeButton, (s, e) =>
			{
				dialog.Cancel();
				dialog.Dismiss();

				if (negative != null)
					negative(); 
			});

			dialog = builder.Show();

			return dialog;
		}

		public static AlertDialog ShowInformation(this Activity activity, string title, string message, string buttonText, Action button = null)
		{
			AlertDialog dialog = null;

			var builder = new AlertDialog.Builder(activity);
			builder.SetTitle(title);
			builder.SetMessage(message);
			
			builder.SetNegativeButton(buttonText, (s, e) =>
			{
				dialog.Cancel();
				dialog.Dismiss();

				if (button != null)
					button();
			});

			dialog = builder.Show();

			return dialog;
		}
	}
}