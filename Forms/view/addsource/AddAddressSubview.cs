using Xamarin.Forms;
using MyCryptos.view.components;
using System;
using System.Collections.Generic;
using System.Linq;
using MyCryptos.Core.Account.Repositories.Base;
using MyCryptos.Core.Account.Repositories.Implementations;
using MyCryptos.Core.Currency.Model;
using MyCryptos.Forms.Resources;
using ZXing.Net.Mobile.Forms;
using ZXing.Mobile;
using ZXing;

namespace MyCryptos.view.addrepositoryviews
{
	public class AddAddressSubview : AddRepositorySubview
	{
		private readonly List<Func<string, Currency, string, AddressAccountRepository>> availableRepositories;
		private readonly IEnumerable<Currency> supportedCurrencies;

		private readonly TableSection section;
		private readonly CurrencyEntryCell currencyEntryCell;
		private readonly CustomEntryCell addressEntryCell;
		private readonly CustomViewCell scanActionCell;

		public AddAddressSubview(INavigation navigation)
		{
			availableRepositories = new List<Func<string, Currency, string, AddressAccountRepository>> {
				(name, coin, address) => new BlockchainAccountRepository(default(int), name, address),
				(name, coin, address) => new EthereumAccountRepository(default(int), name, address),
				(name, coin, address) => new BlockExpertsAccountRepository(default(int), name, coin, address),
				(name, coin, address) => new CryptoIdAccountRepository(default(int), name, coin, address)
			};

			supportedCurrencies = availableRepositories.SelectMany(a => a(null, null, null).SupportedCurrencies);

			currencyEntryCell = new CurrencyEntryCell(navigation) { IsAmountEnabled = false, CurrenciesToSelect = supportedCurrencies, IsFormRepresentation = true };
			addressEntryCell = new CustomEntryCell { Title = I18N.Address, Placeholder = I18N.Address };
			scanActionCell = new CustomViewCell { Text = I18N.ScanQrCode, IsActionCell = true, IsCentered = true };

			section = new TableSection();

			section.Title = I18N.AccountInformation;
			section.Add(currencyEntryCell);
			section.Add(addressEntryCell);
			section.Add(scanActionCell);

			currencyEntryCell.OnSelected = (c) => NameChanged();
			scanActionCell.Tapped += (sender, e) =>
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
						if (result.Text.Contains(':'))
						{
							var split = result.Text.Split(':');
							currencyEntryCell.SelectedCurrency = supportedCurrencies.FirstOrDefault(c => c.Name.ToLower().Equals(split[0])) ?? currencyEntryCell.SelectedCurrency;
							addressEntryCell.Text = split[1];
						}
						else
						{
							addressEntryCell.Text = result.Text;
						}
						NameChanged();
						navigation.PopAsync();
					});
				};
				navigation.PushAsync(scanPage);
			};
		}

		public override OnlineAccountRepository GetRepository(string name)
		{
			var coin = currencyEntryCell.SelectedCurrency;
			var address = addressEntryCell.Text ?? string.Empty;

			var repository = availableRepositories.Select(a => a(name, coin, address)).FirstOrDefault(r => r.SupportedCurrencies.Contains(coin));
			return repository;
		}

		public override bool Enabled
		{
			set
			{
				currencyEntryCell.IsEditable = value;
				addressEntryCell.IsEditable = value;
			}
		}

		public override TableSection InputSection
		{
			get { return section; }
		}

		public sealed override string DefaultName
		{
			get { return GetRepository(null)?.Description ?? I18N.Unnamed; }
		}

		public sealed override string Description
		{
			get { return I18N.Address; }
		}

		public override void Unfocus()
		{
			currencyEntryCell.Unfocus();
			addressEntryCell.Entry.Unfocus();
		}
	}
}
