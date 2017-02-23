using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.Serialization;
using MyCC.Core.Account.Models.Base;
using MyCC.Core.Account.Storage;
using MyCC.Core.Currency.Model;
using MyCC.Core.Resources;
using MyCC.Core.Settings;
using MyCC.Core.Types;
using MyCC.Forms.Constants;
using MyCC.Forms.Messages;
using MyCC.Forms.Resources;
using MyCC.Forms.view.components.CellViews;
using MyCC.Forms.View.Pages;
using Xamarin.Forms;
using XLabs.Forms.Controls;
using XLabs.Ioc;
using XLabs.Serialization;
using XLabs.Serialization.JsonNET;

namespace MyCC.Forms.View.Components
{
    public class AccountsTableComponent : ContentView
    {
        private readonly HybridWebView _webView;
        private readonly Currency _currency;
        private bool _appeared;

        public AccountsTableComponent(INavigation navigation, Currency currency)
        {
            _currency = currency;

            var resolverContainer = new SimpleContainer();

            resolverContainer.Register<IJsonSerializer, JsonSerializer>();

            _webView = new HybridWebView
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                BackgroundColor = Color.White,
                HeightRequest = 0
            };
            _webView.RegisterCallback("Callback", idString =>
            {
                var id = int.Parse(idString);
                var account = AccountStorage.Instance.AllElements.Find(a => a.Id.Equals(id));

                Device.BeginInvokeOnMainThread(() => navigation.PushAsync(new AccountDetailView(account)));
            });

            _webView.RegisterCallback("CallbackSizeAllocated", sizeString =>
            {
                var size = int.Parse(sizeString);
                Device.BeginInvokeOnMainThread(() => _webView.HeightRequest = size);
            });

            _webView.RegisterCallback("HeaderClickedCallback", type =>
            {
                SortOrder value;
                var clickedSortOrder = Enum.TryParse(type, out value) ? value : SortOrder.None;
                if (clickedSortOrder == ApplicationSettings.SortOrderAccounts)
                {
                    ApplicationSettings.SortDirectionAccounts = ApplicationSettings.SortDirectionAccounts == SortDirection.Ascending
                        ? SortDirection.Descending
                        : SortDirection.Ascending;
                }
                ApplicationSettings.SortOrderAccounts = clickedSortOrder;

                UpdateView();
            });

            var stack = new StackLayout { Spacing = 0, HorizontalOptions = LayoutOptions.FillAndExpand, VerticalOptions = LayoutOptions.FillAndExpand, BackgroundColor = AppConstants.TableBackgroundColor };

            stack.Children.Add(new SectionHeaderView(false) { Title = I18N.Accounts });
            stack.Children.Add(_webView);

            Content = stack;

            UpdateView();

            Messaging.UpdatingAccountsAndRates.SubscribeFinished(this, UpdateView);
            Messaging.UpdatingAccounts.SubscribeFinished(this, UpdateView);
        }

        public void OnAppearing()
        {
            if (_appeared) return;

            _appeared = true;
            _webView.LoadFromContent("Html/accountsTable.html");
            _webView.LoadFinished = (sender, e) => UpdateView();
        }

        private void UpdateView()
        {
            try
            {
                var items = AccountStorage.AccountsWithCurrency(_currency).Where(a => a.IsEnabled).Select(a => new Data(a)).ToList();
                var itemsDisabled = AccountStorage.AccountsWithCurrency(_currency).Where(a => !a.IsEnabled).Select(a => new Data(a)).ToList();

                var itemsExisting = items.Any() || itemsDisabled.Any();

                if (!itemsExisting || !_appeared) return;

                Func<Data, object> sortLambda;
                switch (ApplicationSettings.SortOrderAccounts)
                {
                    case SortOrder.Alphabetical: sortLambda = d => d.Name; break;
                    case SortOrder.ByUnits: sortLambda = d => decimal.Parse(d.Amount.Replace("<", "").Trim()); break;
                    case SortOrder.ByValue: sortLambda = d => decimal.Parse(d.Amount.Replace("<", "").Trim()); break;
                    case SortOrder.None: sortLambda = d => 1; break;
                    default: sortLambda = d => 1; break;
                }

                items = ApplicationSettings.SortDirectionAccounts == SortDirection.Ascending ? items.OrderBy(sortLambda).ToList() : items.OrderByDescending(sortLambda).ToList();
                itemsDisabled = ApplicationSettings.SortDirectionAccounts == SortDirection.Ascending ? itemsDisabled.OrderBy(sortLambda).ToList() : itemsDisabled.OrderByDescending(sortLambda).ToList();

                Device.BeginInvokeOnMainThread(() =>
                {
                    _webView.CallJsFunction("setHeader", new[]{
                    new HeaderData(I18N.Label, SortOrder.Alphabetical.ToString()),
                    new HeaderData(I18N.Amount, SortOrder.ByUnits.ToString())
                        }, string.Empty);
                    _webView.CallJsFunction("updateTable", items.Concat(itemsDisabled).ToArray(), new SortData(), DependencyService.Get<ILocalise>().GetCurrentCultureInfo().Name);
                });
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }

        [DataContract]
        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
        [SuppressMessage("ReSharper", "NotAccessedField.Global")]
        public class Data
        {
            [DataMember]
            public readonly string Name;
            [DataMember]
            public readonly string Amount;
            [DataMember]
            public readonly bool Disabled;
            [DataMember]
            public readonly int Id;

            public Data(Account account)
            {
                Name = account.Name;
                Amount = account.Money.ToString8Digits(false);
                Id = account.Id;
                Disabled = !account.IsEnabled;
            }
        }

        [DataContract]
        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
        [SuppressMessage("ReSharper", "NotAccessedField.Global")]
        public class SortData
        {
            [DataMember]
            public readonly string Direction;
            [DataMember]
            public readonly string Type;

            public SortData()
            {
                Direction = ApplicationSettings.SortDirectionAccounts.ToString();
                Type = ApplicationSettings.SortOrderAccounts.ToString();
            }

        }

        [DataContract]
        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
        [SuppressMessage("ReSharper", "NotAccessedField.Global")]
        public class HeaderData
        {
            [DataMember]
            public readonly string Text;
            [DataMember]
            public readonly string Type;

            public HeaderData(string text, string type)
            {
                Text = text;
                Type = type;
            }

        }
    }
}
