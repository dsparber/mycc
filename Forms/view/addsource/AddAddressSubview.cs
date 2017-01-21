using System;
using System.Collections.Generic;
using System.Linq;
using MyCC.Core.Account.Helper;
using MyCC.Core.Account.Repositories.Base;
using MyCC.Core.Account.Repositories.Implementations;
using MyCC.Core.Currency.Model;
using MyCC.Forms.Resources;
using MyCC.Forms.view.components.cells;
using Xamarin.Forms;
using ZXing;
using ZXing.Mobile;
using ZXing.Net.Mobile.Forms;

namespace MyCC.Forms.view.addsource
{
    public class AddAddressSubview : AddRepositorySubview
    {
        private readonly List<Func<string, Currency, string, AddressAccountRepository>> availableRepositories;
        private readonly IEnumerable<Currency> supportedCurrencies;

        private readonly List<TableSection> sections;
        private readonly CurrencyEntryCell currencyEntryCell;
        private readonly CustomEntryCell addressEntryCell;

        public AddAddressSubview(INavigation navigation, Entry nameEntry)
        {
            availableRepositories = new List<Func<string, Currency, string, AddressAccountRepository>> {
                (name, coin, address) => new BlockchainAccountRepository(default(int), name, address),
                (name, coin, address) => new EthereumAccountRepository(default(int), name, address),
                (name, coin, address) => new BlockExpertsAccountRepository(default(int), name, coin, address),
                (name, coin, address) => new CryptoIdAccountRepository(default(int), name, coin, address)
            };

            supportedCurrencies = availableRepositories.SelectMany(a => a(null, null, null).SupportedCurrencies);

            currencyEntryCell = new CurrencyEntryCell(navigation) { IsAmountEnabled = false, CurrenciesToSelect = supportedCurrencies, IsFormRepresentation = true };
            addressEntryCell = new CustomEntryCell { Title = I18N.Address, Placeholder = I18N.AddressOrXpub };
            var scanActionCell = new CustomViewCell { Text = I18N.ScanQrCode, IsActionCell = true, IsCentered = true };

            var sectionQr = new TableSection();
            var sectionInfo = new TableSection { Title = I18N.AccountInformation };

            sectionQr.Add(scanActionCell);
            sectionInfo.Add(addressEntryCell);
            sectionInfo.Add(currencyEntryCell);

            sections = new List<TableSection> { sectionQr, sectionInfo };

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

                    var scanPage = new ZXingScannerPage(options) { DefaultOverlayShowFlashButton = false };

                    scanPage.OnScanResult += (result) =>
                    {
                        scanPage.IsScanning = false;

                        Device.BeginInvokeOnMainThread(() =>
                        {
                            var values = result.Text.Parse(supportedCurrencies);

                            addressEntryCell.Text = values.Item1 ?? addressEntryCell.Text;
                            currencyEntryCell.SelectedCurrency = values.Item2 ?? currencyEntryCell.SelectedCurrency;
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
                    System.Diagnostics.Debug.WriteLine(ex);
                }
            };
        }

        public override OnlineAccountRepository GetRepository(string name)
        {
            var coin = currencyEntryCell.SelectedCurrency;
            var address = addressEntryCell.Text ?? string.Empty;

            if (Currency.Btc.Equals(coin) && address.StartsWith("xpub", StringComparison.CurrentCultureIgnoreCase))
            {
                return new BlockchainXpubAccountRepository(default(int), name, address);
            }
            return availableRepositories.Select(a => a(name, coin, address)).FirstOrDefault(r => r.SupportedCurrencies.Contains(coin));
        }

        public override bool Enabled
        {
            set
            {
                currencyEntryCell.IsEditable = value;
                addressEntryCell.IsEditable = value;
            }
        }

        public override List<TableSection> InputSections => sections;

        public sealed override string Description => I18N.Address;

        public override void Unfocus()
        {
            currencyEntryCell.Unfocus();
            addressEntryCell.Entry.Unfocus();
        }
    }
}
