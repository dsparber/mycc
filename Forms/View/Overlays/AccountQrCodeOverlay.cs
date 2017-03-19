using System.Linq;
using MyCC.Core.Account.Repositories.Base;
using MyCC.Forms.Constants;
using MyCC.Forms.Helpers;
using MyCC.Forms.Resources;
using MyCC.Forms.View.Components;
using Xamarin.Forms;
using XLabs.Forms.Controls;
using XLabs.Ioc;
using XLabs.Serialization;
using XLabs.Serialization.JsonNET;


namespace MyCC.Forms.View.Overlays
{
    public class AccountQrCodeOverlay : ContentPage
    {
        private bool _appeared;
        private readonly HybridWebView _webView;
        private readonly AddressAccountRepository _accountRepository;

        public AccountQrCodeOverlay(AddressAccountRepository accountRepository)
        {
            ToolbarItems.Add(new ToolbarItem { Text = I18N.Cancel });
            ToolbarItems[0].Clicked += (s, e) => Navigation.PopOrPopModal();

            _accountRepository = accountRepository;

            Title = I18N.QrCode;
            BackgroundColor = AppConstants.TableBackgroundColor;

            var resolverContainer = new SimpleContainer();
            resolverContainer.Register<IJsonSerializer, JsonSerializer>();

            _webView = new HybridWebView
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                BackgroundColor = AppConstants.TableBackgroundColor
            };


            var stack = new StackLayout
            {
                Spacing = 0,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                Margin = 15
            };

            stack.Children.Add(_webView);
            stack.Children.Add(new Label
            {
                Text = $"{I18N.Address}: {accountRepository.Address}",
                TextColor = AppConstants.TableSectionColor,
                FontSize = AppConstants.TableSectionFontSize,
                HorizontalTextAlignment = TextAlignment.Center
            });


            var changingStack = new ChangingStackLayout();
            changingStack.Children.Add(new HeaderView
            {
                TitleText = accountRepository.Elements.First().Money.Currency.Name,
                InfoText = accountRepository.Name
            });
            changingStack.Children.Add(stack);

            Content = changingStack;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (_appeared) return;

            _appeared = true;
            _webView.LoadFromContent("Html/qrCode.html");
            _webView.LoadFinished = (sender, e) => _webView.CallJsFunction("setCode", $"{_accountRepository.Currency.Code.ToLower()}:{_accountRepository.Address}?label={_accountRepository.Name}");
        }
    }
}

