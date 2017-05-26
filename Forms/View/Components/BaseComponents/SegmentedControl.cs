using System;
using System.Collections.Generic;
using MyCC.Forms.Constants;
using Xamarin.Forms;

namespace MyCC.Forms.View.Components.BaseComponents
{
    public class SegmentedControl : ContentView
    {
        public Action<int> SelectionChanged;

        private int _selectedIndex;

        public int SelectedIndex
        {
            set { _selectedIndex = value; UpdateView(); }
            get { return _selectedIndex; }
        }

        private readonly StackLayout _stack;

        private List<string> _tabs;

        public List<string> Tabs
        {
            set { _tabs = value; UpdateView(); }
        }

        private Color _backgroundColor;
        public new Color BackgroundColor
        {
            get { return _backgroundColor; }
            set { _backgroundColor = value; UpdateView(); }
        }

        public SegmentedControl()
        {
            _tabs = new List<string>();
            _selectedIndex = 0;

            _stack = new StackLayout { Orientation = StackOrientation.Horizontal, HorizontalOptions = LayoutOptions.Center, Spacing = 1 };
            var frame = new Frame { Content = _stack, OutlineColor = AppConstants.ThemeColor, BackgroundColor = AppConstants.ThemeColor, HasShadow = false, Padding = 0, HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center };
            if (Device.RuntimePlatform.Equals(Device.Android))
            {
                frame.Padding = 1;
            }
            Content = new ContentView { Content = frame, BackgroundColor = BackgroundColor, Padding = 10, HorizontalOptions = LayoutOptions.FillAndExpand };

            BackgroundColor = AppConstants.BackgroundColor;
            UpdateView();
        }

        public void UpdateView()
        {
            _stack.Children.Clear();

            var i = 0;
            foreach (var t in _tabs)
            {
                var selected = _selectedIndex == i;
                var last = i == _tabs.Count - 1;
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
                _stack.Children.Add(border);

                var gestureRecognizer = new TapGestureRecognizer();
                gestureRecognizer.Tapped += (sender, e) =>
                {
                    var title = ((sender as ContentView)?.Content as Label)?.Text;
                    _selectedIndex = _tabs.IndexOf(title);
                    SelectionChanged(_selectedIndex);
                    UpdateView();
                };

                view.GestureRecognizers.Add(gestureRecognizer);

                i = i + 1;
            }

            Content.BackgroundColor = BackgroundColor;
        }
    }
}
