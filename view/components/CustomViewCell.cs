﻿using constants;
using MyCryptos.resources;
using Xamarin.Forms;

namespace MyCryptos.view.components
{
    public class CustomViewCell : SortableViewCell
    {
        protected readonly Label MasterLabel;
        protected readonly Label DetailLabel;
        protected readonly StackLayout LoadingView;
        readonly StackLayout stack;
        protected readonly Image AccessoryImage;

        string text { get; set; }
        string detail { get; set; }
        string image { get; set; }
        bool isLoading { get; set; }

        public string Text
        {
            get { return text; }
            set { text = value; MasterLabel.Text = text; }
        }
        public string Detail
        {
            get { return detail; }
            set { detail = value; DetailLabel.Text = detail; }
        }

        public string Image
        {
            get { return image; }
            set { image = value; AccessoryImage.Source = ImageSource.FromFile(image); }
        }

        public bool IsLoading
        {
            get { return isLoading; }
            set { isLoading = value; DetailLabel.IsVisible = !value; LoadingView.IsVisible = value; }
        }

        public bool IsActionCell
        {
            get { return MasterLabel.TextColor.Equals(AppConstants.ThemeColor); }
            set { MasterLabel.TextColor = value ? AppConstants.ThemeColor : Color.Black; if (value) stack.Children.Remove(DetailLabel); }
        }

        public CustomViewCell()
        {
            MasterLabel = new Label { TextColor = Color.FromHex("222") };

            if (Device.OS == TargetPlatform.Android)
            {
                MasterLabel.FontSize = AppConstants.AndroidFontSize;
            }

            DetailLabel = new Label { TextColor = Color.Gray, FontSize = MasterLabel.FontSize * 0.75 };

            LoadingView = new StackLayout { Orientation = StackOrientation.Horizontal, Spacing = 0, Padding = new Thickness(0), Margin = new Thickness(0) };
            LoadingView.Children.Add(new Label { Text = InternationalisationResources.RefreshingDots, TextColor = Color.Gray, FontSize = MasterLabel.FontSize * 0.75, VerticalOptions = LayoutOptions.Center });

            stack = new StackLayout { Spacing = 0 };
            stack.Children.Add(MasterLabel);
            stack.Children.Add(DetailLabel);
            stack.Children.Add(LoadingView);

            AccessoryImage = new Image { HeightRequest = 20, HorizontalOptions = LayoutOptions.EndAndExpand };

            var mainView = new StackLayout { Orientation = StackOrientation.Horizontal, Padding = new Thickness(15, 0), VerticalOptions = LayoutOptions.CenterAndExpand };
            mainView.Children.Add(stack);
            mainView.Children.Add(AccessoryImage);


            if (Device.OS == TargetPlatform.Android)
            {
                var view = new ContentView { Content = mainView, VerticalOptions = LayoutOptions.FillAndExpand, HorizontalOptions = LayoutOptions.FillAndExpand, BackgroundColor = Color.White };
                View = new ContentView { Content = view, BackgroundColor = Color.FromHex("c7d7d4"), Padding = new Thickness(0, 0, 0, 0.5) };
            }
            else
            {
                View = mainView;
            }

            IsLoading = false;
        }

        public override decimal Units { get { return 0; } }
        public override string Name { get { return Text; } }
        public override decimal Value { get { return 0; } }
    }
}