using System.IO;
using MyCC.Forms.Constants;
using MyCC.Forms.Helpers;
using MyCC.Forms.Resources;
using MyCC.Forms.View.Components.Header;
using MyCC.Forms.View.Container;
using MyCC.Forms.View.Overlays;
using Xamarin.Forms;

namespace MyCC.Forms.View.Components.BaseComponents
{
	public class WebContentView : ContentPage
	{
		private bool _modalOpen;

		public WebContentView(string inputSource, bool local = false)
		{
			var source = local ? Path.Combine(DependencyService.Get<IBaseUrl>().Get(), inputSource) : inputSource;

			var webView = new WebView
			{
				VerticalOptions = LayoutOptions.FillAndExpand,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				Source = source,
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
				if (webView.IsVisible) return;

				webView.IsVisible = true;
				indicator.IsVisible = false;
				indicator.IsRunning = false;
			};

			webView.Navigating += (sender, e) =>
			{
				if (webView.IsVisible)
				{
					e.Cancel = true;
				}

				if (!webView.IsVisible || _modalOpen) return;
				_modalOpen = true;

				Navigation.PushModalAsync(new NavigationPage(new WebOverlay(e.Url)));
			};
		}


		protected override void OnAppearing()
		{
			base.OnAppearing();
			_modalOpen = false;
		}
	}
}