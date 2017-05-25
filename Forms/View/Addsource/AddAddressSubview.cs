using System;
using System.Collections.Generic;
using MyCC.Core.Account.Helper;
using MyCC.Core.Account.Repositories.Base;
using MyCC.Core.Currencies.Model;
using MyCC.Core.Helpers;
using MyCC.Forms.Resources;
using MyCC.Forms.View.Components.Cells;
using Xamarin.Forms;
using ZXing;
using ZXing.Mobile;
using ZXing.Net.Mobile.Forms;

namespace MyCC.Forms.View.Addsource
{
    public class AddAddressSubview : AddRepositorySubview
    {
        private readonly CurrencyEntryCell _currencyEntryCell;
        private readonly CustomEntryCell _addressEntryCell;

        public AddAddressSubview(INavigation navigation, Entry nameEntry)
        {
            var currencyTask = new Func<IEnumerable<Currency>>(() => AddressAccountRepository.AllSupportedCurrencies);

            _currencyEntryCell = new CurrencyEntryCell(navigation, currencyTask) { IsAmountEnabled = false, IsFormRepresentation = true };
            _addressEntryCell = new CustomEntryCell { Title = I18N.Address, Placeholder = I18N.Address };
            var scanActionCell = new CustomViewCell { Text = I18N.ScanQrCode, IsActionCell = true, IsCentered = true };

            var sectionQr = new TableSection();
            var sectionInfo = new TableSection { Title = I18N.Details };

            sectionQr.Add(scanActionCell);
            sectionInfo.Add(_addressEntryCell);
            sectionInfo.Add(_currencyEntryCell);

            InputSections = new List<TableSection> { sectionQr, sectionInfo };

            scanActionCell.Tapped += (sender, e) =>
            {
                try
                {
                    var options = new MobileBarcodeScanningOptions
                    {
                        AutoRotate = true,
                        TryHarder = true,
                        PossibleFormats = new List<BarcodeFormat> { BarcodeFormat.QR_CODE }
                    };

                    var customOverlay = new StackLayout
                    {
                        HorizontalOptions = LayoutOptions.FillAndExpand,
                        VerticalOptions = LayoutOptions.FillAndExpand
                    };
                    var torchOn = false;
                    var torch = new Image
                    {
                        Source = "flashOff.png",
                        Margin = 20,
                        WidthRequest = 40,
                        VerticalOptions = LayoutOptions.Start,
                        HorizontalOptions = LayoutOptions.EndAndExpand
                    };

                    customOverlay.Children.Add(torch);

                    var scanPage = new ZXingScannerPage(options, customOverlay) { DefaultOverlayShowFlashButton = false };

                    var recognizer = new TapGestureRecognizer();
                    recognizer.Tapped += (s, ev) =>
                    {
                        torchOn = !torchOn;
                        torch.Source = torchOn ? "flashOn.png" : "flashOff.png";
                        scanPage.ToggleTorch();
                    };
                    torch.GestureRecognizers.Add(recognizer);

                    scanPage.OnScanResult += result =>
                    {
                        scanPage.IsScanning = false;

                        Device.BeginInvokeOnMainThread(() =>
                        {
                            var values = result.Text.Parse(AddressAccountRepository.AllSupportedCurrencies);

                            _addressEntryCell.Text = values.Item1 ?? _addressEntryCell.Text;
                            _currencyEntryCell.SelectedCurrency = values.Item2 ?? _currencyEntryCell.SelectedCurrency;
                            if (string.IsNullOrEmpty(nameEntry.Text) && values.Item3 != null)
                            {
                                nameEntry.Text = values.Item3;
                            }

                            navigation.PopAsync();
                        });
                    };
                    navigation.PushAsync(scanPage);
                }
                catch (Exception ex)
                {
                    ex.LogError();
                }
            };
        }

        public override OnlineAccountRepository GetRepository(string name)
        {
            var coin = _currencyEntryCell.SelectedCurrency;
            var address = _addressEntryCell.Text ?? string.Empty;

            return AddressAccountRepository.CreateAddressAccountRepository(name, coin, address);
        }

        public override bool Enabled
        {
            set
            {
                _currencyEntryCell.IsEditable = value;
                _addressEntryCell.IsEditable = value;
            }
        }

        public override List<TableSection> InputSections { get; }

        public sealed override string Description => I18N.Address;

        public override void Unfocus()
        {
            _currencyEntryCell.Unfocus();
            _addressEntryCell.Entry.Unfocus();
        }
    }
}
