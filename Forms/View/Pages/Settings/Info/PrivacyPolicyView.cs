using MyCC.Forms.Constants;
using MyCC.Forms.Resources;
using MyCC.Forms.View.Components.Header;
using MyCC.Forms.View.Container;
using Xamarin.Forms;

namespace MyCC.Forms.View.Pages.Settings.Info
{
    public class PrivacyPolicyView : ContentPage
    {
        public PrivacyPolicyView()
        {
            var webView = new WebView
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Source = "https://www.iubenda.com/privacy-policy/8085117",
                IsVisible = false
            };

            var indicator = new ActivityIndicator
            {
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                IsRunning = true
            };

            var stack = new ChangingStackLayout { VerticalOptions = LayoutOptions.FillAndExpand, HorizontalOptions = LayoutOptions.FillAndExpand };
            stack.Children.Add(new HeaderView
            {
                TitleText = I18N.AppName,
                InfoText = $"{I18N.Version} {Core.Settings.Constants.AppVersion}"
            });
            stack.Children.Add(webView);
            stack.Children.Add(indicator);
            Content = stack;
            BackgroundColor = AppConstants.TableBackgroundColor;
            Title = I18N.Privacy;

            webView.Navigated += (sender, args) =>
            {
                webView.IsVisible = true;
                indicator.IsVisible = false;
                indicator.IsRunning = false;
            };
        }
    }
}