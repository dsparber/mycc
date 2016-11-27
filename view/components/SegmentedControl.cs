using constants;
using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace MyCryptos.view.components
{
	public class SegmentedControl : ContentView
	{
		public Action<int> SelectionChanged;

		private int selectedIndex;

		public int SelectedIndex
		{
			set { selectedIndex = value; UpdateView(); }
		}

		private readonly StackLayout stack;

		private List<string> tabs;

		public List<string> Tabs
		{
			set { tabs = value; UpdateView(); }
		}

		public Color backgroundColor;
		public new Color BackgroundColor
		{
			get { return backgroundColor; }
			set { backgroundColor = value; UpdateView(); }
		}

		public SegmentedControl()
		{
			tabs = new List<string>();
			selectedIndex = 0;

			stack = new StackLayout { Orientation = StackOrientation.Horizontal, HorizontalOptions = LayoutOptions.Center, Spacing = 1 };
			var frame = new Frame { Content = stack, OutlineColor = AppConstants.ThemeColor, BackgroundColor = AppConstants.ThemeColor, HasShadow = false, Padding = 0, HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center };
			if (Device.OS == TargetPlatform.Android)
			{
				frame.Padding = 1;
			}
			Content = new ContentView { Content = frame, BackgroundColor = BackgroundColor, Padding = 10, HorizontalOptions = LayoutOptions.FillAndExpand };

			BackgroundColor = AppConstants.BackgroundColor;
			UpdateView();
		}

		public void UpdateView()
		{
			stack.Children.Clear();

			var i = 0;
			foreach (var t in tabs)
			{
				var selected = (selectedIndex == i);
				var last = i == (tabs.Count - 1);
				var first = i == 0;

				var label = new Label { Text = t, TextColor = selected ? Color.White : AppConstants.ThemeColor };
				var view = new Frame { Content = label, Padding = new Thickness(10, 5), BackgroundColor = selected ? AppConstants.ThemeColor : Color.White, HasShadow = false };
				var border = new ContentView { Content = view, BackgroundColor = selected ? AppConstants.ThemeColor : Color.White };
				if (first)
				{
					border.Margin = new Thickness(10, 0, 0, 0);
					border.Padding = new Thickness(-10, 0, 0, 0);
				}
				if (last)
				{
					border.Margin = new Thickness(0, 0, 10, 0);
					border.Padding = new Thickness(0, 0, -10, 0);
				}
				stack.Children.Add(border);

				var gestureRecognizer = new TapGestureRecognizer();
				gestureRecognizer.Tapped += (sender, e) =>
				{
					var title = ((sender as ContentView)?.Content as Label)?.Text;
					selectedIndex = tabs.IndexOf(title);
					SelectionChanged(selectedIndex);
					UpdateView();
				};

				view.GestureRecognizers.Add(gestureRecognizer);

				i = i + 1;
			}

			Content.BackgroundColor = BackgroundColor;
		}
	}
}
