using System;
using MyCC.Forms.Constants;
using MyCC.Forms.Resources;
using Xamarin.Forms;

namespace MyCC.Forms.View.Overlays
{
    public class WebOverlay : ContentPage
    {
        public WebOverlay(string url) : this(new Uri(url)) { }

        public WebOverlay(Uri uri)
        {
            var cancel = new ToolbarItem { Text = I18N.Cancel };
            cancel.Clicked += (sender, args) => Navigation.PopModalAsync();
            ToolbarItems.Add(cancel);

            var webView = new WebView
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Source = uri,
                IsVisible = false
            };

            var indicator = new ActivityIndicator
            {
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                IsRunning = true
            };

            var stack = new StackLayout() { VerticalOptions = LayoutOptions.FillAndExpand, HorizontalOptions = LayoutOptions.FillAndExpand };
            stack.Children.Add(webView);
            stack.Children.Add(indicator);
            Content = stack;
            BackgroundColor = AppConstants.TableBackgroundColor;
            Title = uri.Host;

            webView.Navigated += (sender, args) =>
            {
                webView.IsVisible = true;
                indicator.IsVisible = false;
                indicator.IsRunning = false;
            };
        }
    }
}