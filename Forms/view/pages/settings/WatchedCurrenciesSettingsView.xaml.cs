﻿using System;
using System.Collections.Generic;
using System.Linq;
using MyCryptos.Core.Currency.Model;
using MyCryptos.Core.Currency.Storage;
using MyCryptos.Core.settings;
using MyCryptos.Forms.Messages;
using MyCryptos.Forms.Resources;
using MyCryptos.Forms.view.overlays;
using MyCryptos.view.components;
using MyCryptos.view.components.cells;
using Xamarin.Forms;

namespace MyCryptos.Forms.view.pages.settings
{
	public partial class WatchedCurrenciesSettingsView
	{
		public WatchedCurrenciesSettingsView()
		{
			InitializeComponent();
			SetReferenceCurrencyCells();
		}

		private void SetReferenceCurrencyCells()
		{
			var currencyCells = new List<CustomViewCell>();

			var watchedCurrencies = new List<Currency>(ApplicationSettings.WatchedCurrencies);

			foreach (var c in watchedCurrencies)
			{
				var delete = new CustomViewCellActionItem { Icon = "delete.png", Data = c };
				var items = new List<CustomViewCellActionItem> { delete };

				delete.Action = (sender, e) =>
				{
					var cu = (e as TappedEventArgs)?.Parameter as Currency;
					watchedCurrencies.Remove(cu);
					ApplicationSettings.WatchedCurrencies = watchedCurrencies;
					Messaging.ReferenceCurrencies.SendValueChanged();
					SetReferenceCurrencyCells();
				};

				var cell = new CustomViewCell
				{
					Text = c.Code,
					ActionItems = items,
					Detail = c.Name
				};
				currencyCells.Add(cell);
			}

			AllCurrenciesSection.Clear();
			AllCurrenciesSection.Add(currencyCells);
		}

		private void Add(object sender, EventArgs args)
		{
			var allReferenceCurrencies = ApplicationSettings.WatchedCurrencies.ToArray();
			var currencies = CurrencyStorage.Instance.AllElements.Where(c => !allReferenceCurrencies.Contains(c)).ToList();

			var overlay = new CurrencyOverlay(currencies)
			{
				CurrencySelected = (c) =>
				{
					var watchedCurrencies = new List<Currency>(ApplicationSettings.WatchedCurrencies);
					watchedCurrencies.Add(c);
					ApplicationSettings.WatchedCurrencies = watchedCurrencies;
					Messaging.ReferenceCurrencies.SendValueChanged();
					SetReferenceCurrencyCells();
				}
			};

			Navigation.PushAsync(overlay);
		}
	}
}
