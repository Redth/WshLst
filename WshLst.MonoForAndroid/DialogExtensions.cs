using System;
using Android.App;
using Android.Content;

namespace WshLst.MonoForAndroid
{
	public static class DialogExtensions
	{
		public static AlertDialog ShowQuestion(this Activity activity, string title, string message, string positiveButton,
		                                       string negativeButton, Action positive, Action negative)
		{
			AlertDialog dialog = null;

			var builder = new AlertDialog.Builder(activity);
			builder.SetTitle(title);
			builder.SetMessage(message);
			builder.SetPositiveButton(positiveButton, delegate
				{
					if (dialog == null)
						return;

					dialog.Cancel();
					dialog.Dismiss();

					if (positive != null)
						positive();
				});

			builder.SetNegativeButton(negativeButton, (s, e) =>
				{
					if (dialog == null)
						return;

					dialog.Cancel();
					dialog.Dismiss();

					if (negative != null)
						negative();
				});

			dialog = builder.Show();

			return dialog;
		}

		public static AlertDialog ShowInformation(this Activity activity, string title, string message, string buttonText,
		                                          Action button = null)
		{
			AlertDialog dialog = null;

			var builder = new AlertDialog.Builder(activity);
			builder.SetTitle(title);
			builder.SetMessage(message);

			builder.SetNegativeButton(buttonText, (s, e) =>
				{
					if (dialog == null)
						return;

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