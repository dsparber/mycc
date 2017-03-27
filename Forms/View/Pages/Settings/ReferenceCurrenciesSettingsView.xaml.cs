using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyCC.Core.Account.Storage;
using MyCC.Core.Currency.Model;
using MyCC.Core.Currency.Storage;
using MyCC.Core.Settings;
using MyCC.Forms.Helpers;
using MyCC.Forms.Messages;
using MyCC.Forms.Resources;
using MyCC.Forms.Tasks;
using MyCC.Forms.View.Components.CellViews;
using MyCC.Forms.View.Components.Cells;
using MyCC.Forms.View.Overlays;
using Xamarin.Forms;

namespace MyCC.Forms.View.Pages.Settings
{
	public partial class ReferenceCurrenciesSettingsView
	{
		public ReferenceCurrenciesSettingsView()
		{
			InitializeComponent();

			Header.InfoText = PluralHelper.GetTextCurrencies(ApplicationSettings.AllReferenceCurrencies.Count);

			foreach (var currency in ApplicationSettings.AllReferenceCurrencies.OrderBy(c => c.Code))
			{
				var cell = GetCell(currency);
				CurrenciesSection.Add(cell);
			}
		}

		private void Add(object sender, EventArgs args)
		{
			var currenciesTask = new Func<IEnumerable<Currency>>(() => CurrencyStorage.Instance.AllElements.Where(c => !ApplicationSettings.AllReferenceCurrencies.Contains(c)));

			var overlay = new CurrencyOverlay(currenciesTask, I18N.AddReferenceCurrency)
			{
				CurrencySelected = c =>
				{
					ApplicationSettings.FurtherCurrencies = ApplicationSettings.FurtherCurrencies.Concat(new List<Currency> { c }).ToList();
					var index = Math.Min(ApplicationSettings.AllReferenceCurrencies.OrderBy(x => x.Code).ToList().IndexOf(c), 0);
					CurrenciesSection.Insert(index, GetCell(c));
					Messaging.ReferenceCurrencies.SendValueChanged();
					Task.Run(() => AppTaskHelper.FetchMissingRates(AccountStorage.NeededRates));
				}
			};
			Navigation.PushAsync(overlay);
		}

		private CustomViewCell GetCell(Currency currency)
		{
			var cell = new CustomViewCell { Text = currency.Code, Detail = currency.Name };

			var delete = new CustomCellViewActionItem { Icon = "delete.png", Data = cell };
			var star = new CustomCellViewActionItem { Icon = ApplicationSettings.MainCurrencies.Contains(currency) ? "starFilled.png" : "star.png", Data = cell };
			var items = new List<CustomCellViewActionItem> { star, delete };

			delete.Action = (sender, e) =>
			{
				var c = (e as TappedEventArgs)?.Parameter as CustomViewCell;
				CurrenciesSection.Remove(c);
				if (ApplicationSettings.MainCurrencies.Any(x => x.Code.Equals(c?.Text)))
				{
					ApplicationSettings.MainCurrencies = ApplicationSettings.MainCurrencies.Where(x => !x.Code.Equals(c?.Text)).ToList();
				}
				else
				{
					ApplicationSettings.FurtherCurrencies = ApplicationSettings.FurtherCurrencies.Where(x => !x.Code.Equals(c?.Text)).ToList();
				}
				Messaging.ReferenceCurrencies.SendValueChanged();
			};

			star.Action = (sender, e) =>
			{
				var c = (e as TappedEventArgs)?.Parameter as CustomViewCell;
				var cu = ApplicationSettings.AllReferenceCurrencies.Find(x => x.Code.Equals(c?.Text));
				var isMain = ApplicationSettings.MainCurrencies.Contains(cu);

				if (!isMain && ApplicationSettings.MainCurrencies.Count >= 3)
				{
					DisplayAlert(I18N.Error, I18N.OnlyThreeCurrenciesCanBeStared, I18N.Ok);
				}
				else
				{

					var actionItem = c?.ActionItems.Find(x => x.Icon.StartsWith("star", StringComparison.CurrentCulture));
					if (actionItem != null)
					{
						actionItem.Icon = isMain ? "star.png" : "starFilled.png";
						c.ActionItems = c.ActionItems;
					}

					var mainCurrencies = ApplicationSettings.MainCurrencies;
					var furtherCurrencies = ApplicationSettings.FurtherCurrencies;

					if (isMain)
					{
						mainCurrencies.Remove(cu);
						furtherCurrencies.Add(cu);
					}
					else
					{
						mainCurrencies.Add(cu);
						furtherCurrencies.Remove(cu);
					}
					ApplicationSettings.MainCurrencies = mainCurrencies;
					ApplicationSettings.FurtherCurrencies = furtherCurrencies;

					Messaging.ReferenceCurrencies.SendValueChanged();
				}
			};

			if (currency.Equals(Currency.Btc))
			{
				star.Action = (sender, e) => DisplayAlert(I18N.Error, I18N.BitcoinCanNotBeRemoved, I18N.Ok);
				delete.Action = (sender, e) => DisplayAlert(I18N.Error, I18N.BitcoinCanNotBeRemoved, I18N.Ok);
			}

			cell.ActionItems = items;
			return cell;
		}
	}
}