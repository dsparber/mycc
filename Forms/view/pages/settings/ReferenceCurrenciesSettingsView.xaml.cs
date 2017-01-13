﻿using System;
using System.Collections.Generic;
using System.Linq;
using MyCryptos.Core.Currency.Model;
using MyCryptos.Core.Currency.Storage;
using MyCryptos.Core.settings;
using MyCryptos.Forms.helpers;
using MyCryptos.Forms.Messages;
using MyCryptos.Forms.Resources;
using MyCryptos.Forms.view.overlays;
using MyCryptos.view.components;
using MyCryptos.view.components.cells;
using Xamarin.Forms;

namespace MyCryptos.Forms.view.pages.settings
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
			var currencies = CurrencyStorage.Instance.AllElements;
			ApplicationSettings.AllReferenceCurrencies.Select(currencies.Remove);

			var overlay = new CurrencyOverlay(currencies)
			{
				CurrencySelected = (c) =>
				{
					ApplicationSettings.FurtherCurrencies = ApplicationSettings.FurtherCurrencies.Concat(new List<Currency> { c }).ToList();
					var index = ApplicationSettings.AllReferenceCurrencies.OrderBy(x => x.Code).ToList().IndexOf(c);
					CurrenciesSection.Insert(index, GetCell(c));
					Messaging.ReferenceCurrencies.SendValueChanged();
				}
			};
			Navigation.PushAsync(overlay);
		}

		private CustomViewCell GetCell(Currency currency)
		{
			var cell = new CustomViewCell { Text = currency.Code, Detail = currency.Name };

			var delete = new CustomViewCellActionItem { Icon = "delete.png", Data = cell };
			var star = new CustomViewCellActionItem { Icon = ApplicationSettings.MainCurrencies.Contains(currency) ? "starFilled.png" : "star.png", Data = cell };
			var items = new List<CustomViewCellActionItem> { star, delete };

			delete.Action = (sender, e) =>
			{
				var c = (e as TappedEventArgs)?.Parameter as CustomViewCell;
				CurrenciesSection.Remove(c);
				if (ApplicationSettings.MainCurrencies.Contains(new Currency(c.Text)))
				{
					ApplicationSettings.MainCurrencies = ApplicationSettings.MainCurrencies.Where(x => !x.Code.Equals(c.Text)).ToList();
				}
				else {
					ApplicationSettings.FurtherCurrencies = ApplicationSettings.FurtherCurrencies.Where(x => !x.Code.Equals(c.Text)).ToList();
				}
				Messaging.ReferenceCurrencies.SendValueChanged();
			};

			star.Action = (sender, e) =>
			{
				var c = (e as TappedEventArgs)?.Parameter as CustomViewCell;
				var cu = ApplicationSettings.AllReferenceCurrencies.Find(x => x.Code.Equals(c.Text));
				var isMain = ApplicationSettings.MainCurrencies.Contains(cu);

				if (!isMain && ApplicationSettings.MainCurrencies.Count >= 3)
				{
					DisplayAlert(I18N.Error, I18N.OnlyThreeCurrenciesCanBeStared, I18N.Ok);
				}
				else {

					var actionItem = c.ActionItems.Find(x => x.Icon.StartsWith("star", System.StringComparison.CurrentCulture));
					actionItem.Icon = isMain ? "star.png" : "starFilled.png";
					c.ActionItems = c.ActionItems;

					var mainCurrencies = ApplicationSettings.MainCurrencies;
					var furtherCurrencies = ApplicationSettings.FurtherCurrencies;

					if (isMain)
					{
						mainCurrencies.Remove(cu);
						furtherCurrencies.Add(cu);
					}
					else {
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
				star.Action = (sender, e) => DisplayAlert(I18N.Error, I18N.BitcoinCanNotBeUnstared, I18N.Ok);
				delete.Action = (sender, e) => DisplayAlert(I18N.Error, I18N.BitcoinCanNotBeRemoved, I18N.Ok);
			}

			cell.ActionItems = items;
			return cell;
		}
	}
}