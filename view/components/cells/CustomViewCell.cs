using constants;
using MyCryptos.resources;
using MyCryptos.view.components.cells;
using System.Collections.Generic;
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
            set { text = value; masterLabel.Text = text; detailLabel.IsVisible = (detail != null); }
        }
        public string Detail
        {
            set { detail = value; detailLabel.Text = detail; detailLabel.IsVisible = !IsLoading; }
        }

        public string Image
        {
            set { image = value; accessoryImage.Source = ImageSource.FromFile(image); }
        }

        public List<CustomViewCellActionItem> ActionItems
        {
            set { actions = value; SetActionItems(); }
        }

        public bool IsLoading
        {
            get { return isLoading; }
            set { isLoading = value; detailLabel.IsVisible = !value; loadingView.IsVisible = value; }
        }

        public bool IsActionCell
        {
            set { masterLabel.TextColor = value ? AppConstants.ThemeColor : AppConstants.FontColor; if (value) stack.Children.Remove(detailLabel); }
        }

        public bool IsDeleteActionCell
        {
            set { masterLabel.TextColor = value ? Color.Red : AppConstants.FontColor; if (value) stack.Children.Remove(detailLabel); masterLabel.HorizontalOptions = value ? LayoutOptions.CenterAndExpand : LayoutOptions.StartAndExpand; }
        }

        public CustomViewCell()
        {
            masterLabel = new Label { TextColor = Color.FromHex("222"), LineBreakMode = LineBreakMode.TailTruncation };

            if (Device.OS == TargetPlatform.Android)
            {
                masterLabel.FontSize = AppConstants.AndroidFontSize;
            }

            detailLabel = new Label { TextColor = Color.Gray, FontSize = masterLabel.FontSize * 0.75, LineBreakMode = LineBreakMode.TailTruncation };

            loadingView = new StackLayout { Orientation = StackOrientation.Horizontal, Spacing = 0, Padding = new Thickness(0), Margin = new Thickness(0) };
            loadingView.Children.Add(new Label { Text = I18N.RefreshingDots, TextColor = Color.Gray, FontSize = masterLabel.FontSize * 0.75, VerticalOptions = LayoutOptions.Center });

            stack = new StackLayout { Spacing = 0, HorizontalOptions = LayoutOptions.FillAndExpand };
            stack.Children.Add(masterLabel);
            stack.Children.Add(detailLabel);
            stack.Children.Add(loadingView);

            accessoryImage = new Image { HeightRequest = 20, HorizontalOptions = LayoutOptions.End };
            actionItemsStack = new StackLayout { Orientation = StackOrientation.Horizontal, HorizontalOptions = LayoutOptions.End };

            var mainView = new StackLayout { Orientation = StackOrientation.Horizontal, Padding = new Thickness(15, 0), VerticalOptions = LayoutOptions.CenterAndExpand, HorizontalOptions = LayoutOptions.Fill };
            mainView.Children.Add(stack);
            mainView.Children.Add(actionItemsStack);
            mainView.Children.Add(accessoryImage);


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

        private void SetActionItems()
        {

            actionItemsStack.Children.Clear();
            foreach (var a in actions)
            {
                var img = new Image { HeightRequest = 20, Source = ImageSource.FromFile(a.Icon) };
                var gestureRecognizer = new TapGestureRecognizer();
                gestureRecognizer.Tapped += a.Action;
                gestureRecognizer.CommandParameter = a.Data;
                img.GestureRecognizers.Add(gestureRecognizer);
                actionItemsStack.Children.Add(img);
            }
        }

        public override decimal Units => 0;
        public override string Name => Text;
        public override decimal Value => 0;
    }
}