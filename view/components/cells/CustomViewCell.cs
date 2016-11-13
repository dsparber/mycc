using constants;
using MyCryptos.resources;
using MyCryptos.view.components.cells;
using System.Collections.Generic;
using Xamarin.Forms;

namespace MyCryptos.view.components
{
    public class CustomViewCell : SortableViewCell
    {
        protected readonly Label MasterLabel;
        protected readonly Label DetailLabel;
        protected readonly StackLayout LoadingView;
        readonly StackLayout stack;
        readonly StackLayout ActionItemsStack;
        protected readonly Image AccessoryImage;

        string text { get; set; }
        string detail { get; set; }
        string image { get; set; }
        bool isLoading { get; set; }
        List<CustomViewCellActionItem> actions;

        public string Text
        {
            get { return text; }
			set { text = value; MasterLabel.Text = text; DetailLabel.IsVisible = (detail != null); }
        }
        public string Detail
        {
            get { return detail; }
			set { detail = value; DetailLabel.Text = detail; DetailLabel.IsVisible = !IsLoading; }
        }

        public string Image
        {
            get { return image; }
            set { image = value; AccessoryImage.Source = ImageSource.FromFile(image); }
        }

        public List<CustomViewCellActionItem> ActionItems
        {
            get { return actions; }
            set { actions = value; setActionItems(); }
        }

        public bool IsLoading
        {
            get { return isLoading; }
            set { isLoading = value; DetailLabel.IsVisible = !value; LoadingView.IsVisible = value; }
        }

        public bool IsActionCell
        {
            get { return MasterLabel.TextColor.Equals(AppConstants.ThemeColor); }
            set { MasterLabel.TextColor = value ? AppConstants.ThemeColor : AppConstants.FontColor; if (value) stack.Children.Remove(DetailLabel); }
        }

        public bool IsDeleteActionCell
        {
            get { return MasterLabel.TextColor.Equals(Color.Red); }
            set { MasterLabel.TextColor = value ? Color.Red : AppConstants.FontColor; if (value) stack.Children.Remove(DetailLabel); MasterLabel.HorizontalOptions = value ? LayoutOptions.CenterAndExpand : LayoutOptions.StartAndExpand; }
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

            stack = new StackLayout { Spacing = 0, HorizontalOptions = LayoutOptions.FillAndExpand };
            stack.Children.Add(MasterLabel);
            stack.Children.Add(DetailLabel);
            stack.Children.Add(LoadingView);

            AccessoryImage = new Image { HeightRequest = 20, HorizontalOptions = LayoutOptions.End };
            ActionItemsStack = new StackLayout { Orientation = StackOrientation.Horizontal, HorizontalOptions = LayoutOptions.End };

            var mainView = new StackLayout { Orientation = StackOrientation.Horizontal, Padding = new Thickness(15, 0), VerticalOptions = LayoutOptions.CenterAndExpand, HorizontalOptions = LayoutOptions.Fill };
            mainView.Children.Add(stack);
            mainView.Children.Add(ActionItemsStack);
            mainView.Children.Add(AccessoryImage);


            if (Device.OS == TargetPlatform.Android)
            {
                var content = new ContentView { Content = mainView, VerticalOptions = LayoutOptions.FillAndExpand, HorizontalOptions = LayoutOptions.FillAndExpand, BackgroundColor = Color.White };
                View = new ContentView { Content = content, BackgroundColor = Color.FromHex("c7d7d4"), Padding = new Thickness(0, 0, 0, 0.5) };
            }
            else
            {
                View = mainView;
            }

            IsLoading = false;
        }

        void setActionItems() {

            ActionItemsStack.Children.Clear();
            foreach (var a in actions)
            {
                var image = new Image { HeightRequest = 20, Source = ImageSource.FromFile(a.Icon) };
                var gestureRecognizer = new TapGestureRecognizer ();
                gestureRecognizer.Tapped += a.Action;
                gestureRecognizer.CommandParameter = a.Data;
                image.GestureRecognizers.Add(gestureRecognizer);
                ActionItemsStack.Children.Add(image);
            }
        }

        public override decimal Units { get { return 0; } }
        public override string Name { get { return Text; } }
        public override decimal Value { get { return 0; } }
    }
}