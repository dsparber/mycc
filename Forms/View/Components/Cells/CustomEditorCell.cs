using MyCC.Forms.Constants;
using Xamarin.Forms;

namespace MyCC.Forms.View.Components.Cells
{
	public class CustomEditorCell : ViewCell
	{
		public readonly Editor Editor;
		private readonly Label _titleLabel;

		private string _title;
		private string _placeholder;

		public string Title
		{
			set { _title = value; _titleLabel.Text = _title; }
		}

		public string Text
		{
			get { return Editor != null ? Editor.Text : string.Empty; }
			set { Editor.Text = value; }
		}

		public bool IsEditable
		{
			set
			{
				Editor.IsEnabled = value;
				Editor.Opacity = value ? 1 : 0.5;
			}
		}

		public CustomEditorCell()
		{
			Editor = new Editor
			{
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand,
				Margin = new Thickness(0, 6, 6, 6)
			};
			_titleLabel = new Label
			{
				WidthRequest = AppConstants.LabelWidth,
				MinimumWidthRequest = AppConstants.LabelWidth,
				VerticalOptions = LayoutOptions.StartAndExpand,
				Margin = new Thickness(0, 15),
				TextColor = AppConstants.FontColor,
				LineBreakMode = LineBreakMode.NoWrap
			};

			var stack = new StackLayout
			{
				Orientation = StackOrientation.Horizontal,
				Spacing = 0,
				Padding = new Thickness(15, 0),
				VerticalOptions = LayoutOptions.FillAndExpand,
				HorizontalOptions = LayoutOptions.FillAndExpand
			};
			stack.Children.Add(_titleLabel);
			stack.Children.Add(Editor);


			if (Device.OS == TargetPlatform.Android)
			{
				_titleLabel.FontSize = AppConstants.AndroidFontSize;
				Editor.FontSize = AppConstants.AndroidFontSize;
			}

			if (Device.OS == TargetPlatform.Android)
			{
				stack.BackgroundColor = Color.White;
				View = new ContentView { Content = stack, VerticalOptions = LayoutOptions.FillAndExpand, BackgroundColor = Color.FromHex("c7d7d4"), Padding = new Thickness(0, 0, 0, 0.5) };
			}
			else
			{
				View = stack;
			}

			Height = 72;

			var gestureRecogniser = new TapGestureRecognizer();
			gestureRecogniser.Tapped += (sender, e) => Editor.Focus();
			View.GestureRecognizers.Add(gestureRecogniser);
		}
	}
}

