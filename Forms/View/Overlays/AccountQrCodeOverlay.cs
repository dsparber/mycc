using System;
using System.Collections.Generic;
using System.Linq;
using MyCC.Core.Account.Repositories.Base;
using MyCC.Forms.Constants;
using MyCC.Forms.Helpers;
using MyCC.Forms.Resources;
using MyCC.Forms.View.Components.BaseComponents;
using MyCC.Forms.View.Container;
using Xamarin.Forms;
using HeaderView = MyCC.Forms.View.Components.Header.HeaderView;


namespace MyCC.Forms.View.Overlays
{
    public class AccountQrCodeOverlay : ContentPage
    {
        public AccountQrCodeOverlay(AddressAccountRepository accountRepository)
        {
            ToolbarItems.Add(new ToolbarItem { Text = I18N.Cancel });
            ToolbarItems[0].Clicked += (s, e) => Navigation.PopOrPopModal();

            Title = I18N.QrCode;
            BackgroundColor = AppConstants.TableBackgroundColor;

            var segementedControl = new SegmentedControl
            {
                Tabs = new List<string> { I18N.AddressOnly, I18N.AllInfos },
                BackgroundColor = AppConstants.TableBackgroundColor
            };

            Func<string> qrText = () => segementedControl.SelectedIndex == 0
                        ? accountRepository.Address
                        : $"{accountRepository.Currency.Code.ToLower()}:{accountRepository.Address}?label={accountRepository.Name}";

            var webView = new HybridWebView("Html/qrCode.html");
            webView.LoadFinished = () => webView.CallJsFunction("setCode", qrText());

            segementedControl.SelectionChanged = i =>
            {
                webView.CallJsFunction("setCode", qrText());
            };

            var stack = new StackLayout
            {
                Spacing = 0,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                Margin = 15
            };


            stack.Children.Add(segementedControl);
            stack.Children.Add(webView);
            stack.Children.Add(new Label
            {
                Text = $"{I18N.Address}:\n{accountRepository.Address.Substring(0, accountRepository.Address.Length / 2)}\u200B{accountRepository.Address.Substring(accountRepository.Address.Length / 2)}",
                TextColor = AppConstants.TableSectionColor,
                FontSize = AppConstants.TableSectionFontSize,
                HorizontalTextAlignment = TextAlignment.Center
            });


            var changingStack = new ChangingStackLayout();
            changingStack.Children.Add(new HeaderView
            {
                TitleText = accountRepository.Elements.First().Money.Currency.Code,
                InfoText = accountRepository.Name
            });
            changingStack.Children.Add(stack);

            Content = changingStack;
        }
    }
}

