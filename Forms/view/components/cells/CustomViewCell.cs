using constants;
using MyCryptos.view.components.cells;
using System.Collections.Generic;
using MyCryptos.Forms.Resources;
using Xamarin.Forms;

namespace MyCryptos.view.components
{
	public class CustomViewCell : SortableViewCell
	{
		private readonly Label masterLabel;
		private readonly Label detailLabel;
		private readonly StackLayout loadingView;
		private readonly StackLayout stack;
		private readonly StackLayout actionItemsStack;
		private readonly Image accessoryImage;

		private string text;
		private string detail;
		private string image;
		private bool isLoading;
		private List<CustomViewCellActionItem> actions;

		public string Text
		{
			get { return text; }
			set
			{
				text = value;
				Device.BeginInvokeOnMainThread(() =>
				{
					masterLabel.Text = text;
					detailLabel.IsVisible = (detail != null);
				});
			}
		}
		public string Detail
		{
			get { return detail; }
			set
			{
				detail = value;
				Device.BeginInvokeOnMainThread(() =>
				{
					detailLabel.Text = detail;
					detailLabel.IsVisible = !IsLoading;
				});
			}
		}

		public LineBreakMode DetailBreakMode
		{
			set
			{
				Device.BeginInvokeOnMainThread(() =>
				{
					detailLabel.LineBreakMode = value;
				});
			}
		}

		public string Image
		{
			set { image = value; Device.BeginInvokeOnMainThread(() => accessoryImage.Source = ImageSource.FromFile(image)); }
		}

		public bool ShowIcon
		{
			set { Device.BeginInvokeOnMainThread(() => accessoryImage.IsVisible = value); }
			get { return accessoryImage?.IsVisible ?? false; }
		}

		public List<CustomViewCellActionItem> ActionItems
		{
			set { actions = value; SetActionItems(); }
			get { return actions; }
		}

		public bool IsLoading
		{
			get { return isLoading; }
			set
			{
				isLoading = value;
				Device.BeginInvokeOnMainThread(() =>
				{
					detailLabel.IsVisible = !value;
					loadingView.IsVisible = value;
				});
			}
		}

		public bool IsActionCell
		{
			set
			{
				Device.BeginInvokeOnMainThread(() =>
				{
					masterLabel.TextColor = value ? AppConstants.ThemeColor : AppConstants.FontColor;
					if (value) stack.Children.Remove(detailLabel);
				});
			}
		}

		public bool IsDeleteActionCell
		{
			set
			{
				Device.BeginInvokeOnMainThread(() =>
				{
					masterLabel.TextColor = value ? Color.Red : AppConstants.FontColor;
					if (value) stack.Children.Remove(detailLabel);
					masterLabel.HorizontalOptions = value ? LayoutOptions.CenterAndExpand : LayoutOptions.StartAndExpand;
				});
			}
		}

		public bool IsCentered
		{
			set { Device.BeginInvokeOnMainThread(() => masterLabel.HorizontalOptions = value ? LayoutOptions.CenterAndExpand : LayoutOptions.StartAndExpand); }
		}

		public CustomViewCell()
		{
			masterLabel = new Label { TextColor = Color.FromHex("222"), LineBreakMode = LineBreakMode.TailTruncation, VerticalOptions = LayoutOptions.Center };

			if (Device.OS == TargetPlatform.Android)
			{
				masterLabel.FontSize = AppConstants.AndroidFontSize;
			}

			detailLabel = new Label { TextColor = Color.Gray, FontSize = masterLabel.FontSize * 0.75, LineBreakMode = LineBreakMode.MiddleTruncation, VerticalOptions = LayoutOptions.Center };

			loadingView = new StackLayout { Orientation = StackOrientation.Horizontal, Spacing = 0, Padding = new Thickness(0), Margin = new Thickness(0) };
			loadingView.Children.Add(new Label { Text = I18N.RefreshingDots, TextColor = Color.Gray, FontSize = masterLabel.FontSize * 0.75, VerticalOptions = LayoutOptions.Center });

			stack = new StackLayout { Spacing = 0, HorizontalOptions = LayoutOptions.FillAndExpand, VerticalOptions = LayoutOptions.CenterAndExpand };
			stack.Children.Add(masterLabel);
			stack.Children.Add(detailLabel);
			stack.Children.Add(loadingView);

			accessoryImage = new Image { HeightRequest = 20, HorizontalOptions = LayoutOptions.End, VerticalOptions = LayoutOptions.FillAndExpand };
			actionItemsStack = new StackLayout { Orientation = StackOrientation.Horizontal, HorizontalOptions = LayoutOptions.End, VerticalOptions = LayoutOptions.FillAndExpand };

			var mainView = new StackLayout { Orientation = StackOrientation.Horizontal, Padding = new Thickness(15, 0), VerticalOptions = LayoutOptions.FillAndExpand, HorizontalOptions = LayoutOptions.Fill };
			mainView.Children.Add(stack);
			mainView.Children.Add(actionItemsStack);
			mainView.Children.Add(accessoryImage);

			var content = new ContentView { Content = mainView, VerticalOptions = LayoutOptions.FillAndExpand, HorizontalOptions = LayoutOptions.FillAndExpand, BackgroundColor = Color.White };

			View = (Device.OS == TargetPlatform.Android) ? new ContentView { Content = content, BackgroundColor = Color.FromHex("c7d7d4"), Padding = new Thickness(0, 0, 0, 0.5) } : content;

			IsLoading = false;
		}

		private void SetActionItems()
		{
			Device.BeginInvokeOnMainThread(() =>
			{
				actionItemsStack.Children.Clear();
				foreach (var a in actions)
				{
					var img = new Image
					{
						HeightRequest = 20,
						Source = ImageSource.FromFile(a.Icon),
						VerticalOptions = LayoutOptions.Center
					};
					var content = new ContentView
					{
						Content = img,
						Padding = new Thickness(10, 0),
						VerticalOptions = LayoutOptions.FillAndExpand
					};
					var gestureRecognizer = new TapGestureRecognizer();
					gestureRecognizer.Tapped += a.Action;
					gestureRecognizer.CommandParameter = a.Data;
					content.GestureRecognizers.Add(gestureRecognizer);
					actionItemsStack.Children.Add(content);
				}
			});
		}

		public override decimal Units => 0;
		public override string Name => Text;
		public override decimal Value => 0;
	}
}