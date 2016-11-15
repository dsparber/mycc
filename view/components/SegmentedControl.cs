using constants;
using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace MyCryptos.view.components
{
	public class SegmentedControl : ContentView
	{
		public Action<int> SelectionChanged;

		public int SelectedIndex;

		StackLayout stack;

		List<string> tabs;

		public List<string> Tabs
		{
			get { return tabs; }
			set { tabs = value; updateView(); }
		}

		public SegmentedControl()
		{
			tabs = new List<string>();
			SelectedIndex = 0;

			stack = new StackLayout { Orientation = StackOrientation.Horizontal, HorizontalOptions = LayoutOptions.Center, Spacing = 1 };
			var frame = new Frame { Content = stack, OutlineColor = AppConstants.ThemeColor, BackgroundColor = AppConstants.ThemeColor, HasShadow = false, Padding = 0, HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center };
			if (Device.OS == TargetPlatform.Android)
			{
				frame.Padding = 1;
			}
			Content = new ContentView { Content = frame, BackgroundColor = AppConstants.BackgroundColor, Padding = 10 };

			updateView();
		}

		void updateView()
		{
			stack.Children.Clear();

			var i = 0;
			foreach (var t in tabs)
			{
				var selected = (SelectedIndex == i);
				var last = i == (tabs.Count - 1);
				var first = i == 0;

				var label = new Label { Text = t, TextColor = selected ? AppConstants.BackgroundColor : AppConstants.ThemeColor };
				var view = new Frame { Content = label, Padding = new Thickness(10, 5), BackgroundColor = selected ? AppConstants.ThemeColor : AppConstants.BackgroundColor, HasShadow = false };
				var border = new ContentView { Content = view, BackgroundColor = selected ? AppConstants.ThemeColor : AppConstants.BackgroundColor };
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
					var title = ((sender as ContentView).Content as Label).Text;
					SelectedIndex = tabs.IndexOf(title);
					SelectionChanged(SelectedIndex);
					updateView();
				};

				view.GestureRecognizers.Add(gestureRecognizer);

				i = i + 1;
			}
		}
	}
}
