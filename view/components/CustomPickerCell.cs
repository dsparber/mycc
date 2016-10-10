﻿using constants;
using Xamarin.Forms;

namespace MyCryptos.view.components
{
    class CustomPickerCell : ViewCell
    {
        public readonly Picker Picker;
        protected readonly Label TitleLabel;

        string title;
        string placeholder;

        public string Title
        {
            get { return title; }
            set { title = value; TitleLabel.Text = title; }
        }

        public bool IsEditable
        {
            set
            {
                Picker.IsEnabled = value;
                Picker.Opacity = value ? 1 : 0.5;
            }
        }

        public CustomPickerCell()
        {
            Picker = new Picker { HorizontalOptions = LayoutOptions.FillAndExpand, VerticalOptions = LayoutOptions.CenterAndExpand };
            TitleLabel = new Label { WidthRequest = 100, VerticalOptions = LayoutOptions.CenterAndExpand };

            var stack = new StackLayout { Orientation = StackOrientation.Horizontal, Padding = new Thickness(15, 0), VerticalOptions = LayoutOptions.CenterAndExpand };
            stack.Children.Add(TitleLabel);
            stack.Children.Add(Picker);


            if (Device.OS == TargetPlatform.Android)
            {
                TitleLabel.FontSize = AppConstants.AndroidFontSize;
            }

            stack.HorizontalOptions = LayoutOptions.FillAndExpand;
            stack.VerticalOptions = LayoutOptions.FillAndExpand;
            if (Device.OS == TargetPlatform.Android)
            {
                stack.BackgroundColor = Color.White;
                View = new ContentView { Content = stack, BackgroundColor = Color.FromHex("c7d7d4"), Padding = new Thickness(0, 0, 0, 0.5), Margin = new Thickness(0, 0, 0, -0.5) };
            }
            else
            {
                View = stack;
            }

            var gestureRecogniser = new TapGestureRecognizer();
            gestureRecogniser.Tapped += (sender, e) => Picker.Focus();
            View.GestureRecognizers.Add(gestureRecogniser);
        }
    }
}
