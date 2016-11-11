using data.repositories.account;
using data.settings;
using data.storage;
using helpers;
using message;
using MyCryptos.helpers;
using MyCryptos.models;
using MyCryptos.resources;
using MyCryptos.view.components;
using System;
using System.Collections.Generic;
using System.Linq;
using view;
using Xamarin.Forms;

namespace MyCryptos.view
{
    public partial class CoinsTableView : ContentView
    {
        List<SortableViewCell> Cells;

        public CoinsTableView()
        {
            InitializeComponent();

            Cells = new List<SortableViewCell>();
            CoinsSection.Title = InternationalisationResources.Coins;

            MessagingCenter.Subscribe<string>(this, MessageConstants.UpdatedExchangeRates, str => updateView());
            MessagingCenter.Subscribe<string>(this, MessageConstants.UpdatedReferenceCurrency, str => updateView());
            MessagingCenter.Subscribe<string>(this, MessageConstants.UpdatedAccounts, str => updateView());
            MessagingCenter.Subscribe<string>(this, MessageConstants.UpdatedSortOrder, str => SortHelper.ApplySortOrder(Cells, CoinsSection));
        }

        void updateView()
        {
            setCells();
        }

        IEnumerable<IGrouping<Currency, Tuple<Account, AccountRepository>>> groups
        {
            get
            {
                var allAccounts = AccountStorage.Instance.AllElementsWithRepositories;
                return allAccounts.GroupBy(a => a.Item1.Money.Currency);
            }
        }

        void setCells()
        {
            var cells = new List<SortableViewCell>();

            foreach (var g in groups)
            {
                if (g.Key != null)
                {
                    var cell = Cells.OfType<CoinViewCell>().ToList().Find(e => g.Key.Equals(e.Currency));
                    if (cell == null)
                    {
                        cell = new CoinViewCell(Navigation) { Accounts = g.ToList(), IsLoading = true };
                    }
                    else
                    {
                        cell.Accounts = g.ToList();
                    }

                    var neededRate = new ExchangeRate(cell.Currency, ApplicationSettings.BaseCurrency);
                    var rate = ExchangeRateHelper.GetRate(neededRate);
                    cell.ExchangeRate = rate;

                    cell.IsLoading = rate != null && !rate.Rate.HasValue;

                    cells.Add(cell);
                }
            }
            if (cells.Count == 0)
            {
                var localAccountCell = new CustomViewCell { Text = InternationalisationResources.AddLocalAccount, IsActionCell = true };
                localAccountCell.Tapped += (sender, e) => Navigation.PushOrPushModal(new AccountDetailView(null, null) { IsNew = true });
                var addSourceCell = new CustomViewCell { Text = InternationalisationResources.AddSource, IsActionCell = true };
                addSourceCell.Tapped += (sender, e) => Navigation.PushOrPushModal(new AddRepositoryView());
                cells.Add(localAccountCell);
                cells.Add(addSourceCell);
            }

            Cells = cells;
            SortHelper.ApplySortOrder(Cells, CoinsSection);
            
        }
    }
}
