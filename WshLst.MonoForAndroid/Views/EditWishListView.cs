using Android.App;
using Android.Views;
using Android.Widget;
using Cirrious.MvvmCross.Binding.Droid.Views;
using WshLst.Core.ViewModels;

namespace WshLst.MonoForAndroid.Views
{
	[Activity(Label = "New Wish List", Icon = "@drawable/icontransparent")]
	public class EditWishListView : MvxBindingActivityView<EditWishListViewModel>
	{
		private Button _buttonCancel;
		private Button _buttonSave;
		private EditText _textDescription;
		private EditText _textName;

		protected override void OnViewModelSet()
		{
			RequestWindowFeature(WindowFeatures.ActionBar);

			SetContentView(Resource.Layout.Page_EditWishListView);

			_textName = FindViewById<EditText>(Resource.Id.textName);
			_textDescription = FindViewById<EditText>(Resource.Id.textDescription);
			_buttonCancel = FindViewById<Button>(Resource.Id.buttonCancel);
			_buttonSave = FindViewById<Button>(Resource.Id.buttonSave);


			_buttonCancel.Click += (s, e) => ViewModel.Cancel();

			_buttonSave.Click += (s, e) =>
				{
					ViewModel.Name = _textName.Text;
					ViewModel.Description = _textDescription.Text;

					ViewModel.Save();
				};

			ViewModel.PropertyChanged += (s, e) =>
				{
					switch (e.PropertyName)
					{
						case "Name":
							_textName.Text = ViewModel.Name;
							break;
						case "Description":
							_textDescription.Text = ViewModel.Description;
							break;
						case "ViewTitle":
							Title = ViewModel.ViewTitle;
							break;
					}
				};
		}
	}
}