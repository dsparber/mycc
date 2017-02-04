using Xamarin.Forms;
using XLabs.Forms.Controls;
using MyCC.Forms.Resources;
using MyCC.Forms.view.components;
using MyCC.Core.Settings;
using MyCC.Forms.constants;

namespace MyCC.Forms.View.Pages.Settings
{
	public class AboutView : ContentPage
	{
		HybridWebView _webView;

		public AboutView()
		{
			_webView = new HybridWebView { VerticalOptions = LayoutOptions.FillAndExpand, HorizontalOptions = LayoutOptions.FillAndExpand };

			var stack = new StackLayout { Spacing = 0, VerticalOptions = LayoutOptions.FillAndExpand, HorizontalOptions = LayoutOptions.FillAndExpand };
			stack.Children.Add(new HeaderView { TitleText = I18N.AppName, InfoText = $"{I18N.Version} {Constants.AppVersion}" });
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

