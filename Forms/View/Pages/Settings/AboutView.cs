using Xamarin.Forms;
using XLabs.Forms.Controls;
using MyCC.Forms.Resources;
using MyCC.Forms.Constants;
using MyCC.Forms.View.Components;
using HeaderView = MyCC.Forms.View.Components.HeaderView;

namespace MyCC.Forms.View.Pages.Settings
{
    public class AboutView : ContentPage
    {
        private readonly HybridWebView _webView;

        public AboutView()
        {
            _webView = new HybridWebView { VerticalOptions = LayoutOptions.FillAndExpand, HorizontalOptions = LayoutOptions.FillAndExpand };

            var stack = new ChangingStackLayout { VerticalOptions = LayoutOptions.FillAndExpand, HorizontalOptions = LayoutOptions.FillAndExpand };
            stack.Children.Add(new HeaderView { TitleText = I18N.AppName, InfoText = $"{I18N.Version} {Core.Settings.Constants.AppVersion}" });
            stack.Children.Add(_webView);
            Content = stack;
            BackgroundColor = AppConstants.TableBackgroundColor;
            Title = I18N.About;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _webView.LoadFromContent("Html/about.html");
            _webView.CallJsFunction("window.history.back");
        }
    }
}

