using System.Linq;
using MyCC.Core.Account.Repositories.Base;
using MyCC.Core.Settings;
using MyCC.Forms.Constants;
using MyCC.Forms.Helpers;
using MyCC.Forms.Resources;
using MyCC.Forms.View.Components;
using Xamarin.Forms;
using ZXing.Net.Mobile.Forms;

namespace MyCC.Forms.View.Overlays
{
    public class AccountQrCodeOverlay : ContentPage
    {
        ZXingBarcodeImageView _barcodeView;

        public AccountQrCodeOverlay(AddressAccountRepository accountRepository)
        {
            ToolbarItems.Add(new ToolbarItem { Text = I18N.Cancel });
            ToolbarItems.Add(new ToolbarItem { Text = I18N.Done });
            ToolbarItems[0].Clicked += (s, e) => Navigation.PopOrPopModal();
            ToolbarItems[1].Clicked += (s, e) => Navigation.PopOrPopModal();

            Title = accountRepository.Name;
            BackgroundColor = AppConstants.TableBackgroundColor;

            _barcodeView = new ZXingBarcodeImageView { HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.CenterAndExpand, Margin = 25 };
            _barcodeView.BarcodeFormat = ZXing.BarcodeFormat.QR_CODE;
            _barcodeView.BarcodeOptions.Width = 300;
            _barcodeView.BarcodeOptions.Height = 300;
            _barcodeView.BarcodeOptions.Margin = 1;

            _barcodeView.BarcodeValue = $"{accountRepository.Currency.Code.ToLower()}:{accountRepository.Address}?label={accountRepository.Name}";

            var stack = new StackLayout
            {
                Spacing = 0,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                Margin = 15
            };

            stack.Children.Add(_barcodeView);
            stack.Children.Add(new Label { Text = $"{I18N.Address}: {accountRepository.Address}", TextColor = AppConstants.TableSectionColor, FontSize = AppConstants.TableSectionFontSize, HorizontalTextAlignment = TextAlignment.Center });


            var changingStack = new ChangingStackLayout();
            changingStack.Children.Add(new HeaderView
            {
                TitleText = accountRepository.Elements.First().Money.ToStringTwoDigits(ApplicationSettings.RoundMoney),
                InfoText = accountRepository.Elements.First().Money.Currency.Name
            });
            changingStack.Children.Add(stack);



            Content = changingStack;
        }
    }
}

