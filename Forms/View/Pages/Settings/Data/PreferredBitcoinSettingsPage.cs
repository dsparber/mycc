using System;
using System.Collections.Generic;
using System.Linq;
using MyCC.Core;
using MyCC.Core.Resources;
using MyCC.Forms.Constants;
using MyCC.Forms.Helpers;
using MyCC.Forms.Resources;
using MyCC.Forms.View.Components;
using MyCC.Forms.View.Components.CellViews;
using MyCC.Forms.View.Container;
using MyCC.Ui;
using MyCC.Ui.DataItems;
using MyCC.Ui.Messages;
using Refractored.XamForms.PullToRefresh;
using Xamarin.Forms;
using HeaderView = MyCC.Forms.View.Components.Header.HeaderView;

namespace MyCC.Forms.View.Pages.Settings.Data
{
    public class PreferredBitcoinSettingsPage : ContentPage
    {
        private readonly Dictionary<string, CustomCellView> _views;
        private readonly InfoFooterComponent _footer;
        private static bool _triedUpdate;

        public PreferredBitcoinSettingsPage()
        {
            Title = I18N.PreferredBitcoinRate;
            BackgroundColor = AppConstants.TableBackgroundColor;

            var tableView = new StackLayout { Spacing = 0 };
            tableView.Children.Add(new SectionHeaderView { Title = I18N.Sources });

            _views = MyccUtil.Rates.CryptoToFiatSourcesWithDetail.ToList().OrderBy(t => t.name).ToDictionary(t => t.name, t =>
            {
                var cell = new CustomCellView
                {
                    Text = t.name,
                    Detail = t.detail,
                    ShowIcon = t.selected,
                    Image = "checkmark.png"
                };

                var gr = new TapGestureRecognizer();
                gr.Tapped += (sender, args) =>
                {
                    UiUtils.Edit.SelectedCryptoToFiatSource = t.name;
                    Navigation.PopAsync();
                };
                cell.GestureRecognizers.Add(gr);
                tableView.Children.Add(cell);

                return cell;
            });

            _footer = new InfoFooterComponent { Text = MyccUtil.Rates.LastUpdate().LastUpdateString() };
            var header = new HeaderView(true)
            {
                Data = new HeaderItem(ConstantNames.AppNameShort, PluralHelper.GetTextSourcs(MyccUtil.Rates.CryptoToFiatSourceCount))
            };

            var changingStack = new ChangingStackLayout();
            changingStack.Children.Add(header);
            var pullToRefresh = new PullToRefreshLayout
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Content = new ScrollView { Content = tableView },
                BackgroundColor = AppConstants.TableBackgroundColor,
                RefreshCommand = new Command(() => UiUtils.Update.FetchCryptoToFiatRates()),
            };



            changingStack.Children.Add(new StackLayout
            {
                Spacing = 0,
                BackgroundColor = AppConstants.BorderColor,
                Children = {
                            pullToRefresh,
                            new ContentView
                            {
                                Margin = new Thickness(0, 1, 0, 0),
                                Padding = 5,
                                Content = new Label
                                {
                                    Text = $"* {I18N.InfoNoDirectRate}",
                                    VerticalOptions = LayoutOptions.End,
                                    FontSize = 12,
                                    TextColor = AppConstants.FontColorLight
                                },
                                BackgroundColor = Color.White
                            },
                            _footer

                    },
                VerticalOptions = LayoutOptions.FillAndExpand


            });
            Content = changingStack;

            Messaging.Update.CryptoToFiatRates.Subscribe(this, () => Device.BeginInvokeOnMainThread(() =>
            {
                SetFooter();
                Update();
                pullToRefresh.IsRefreshing = false;
            }));
            SetFooter();
        }
        private void SetFooter()
        {
            var lastUpdate = MyccUtil.Rates.LastCryptoToFiatUpdate();
            if (lastUpdate < DateTime.Now.Subtract(new TimeSpan(6, 0, 0)) && !_triedUpdate)
            {
                _triedUpdate = true;
                UiUtils.Update.FetchCryptoToFiatRates();
            }
            _footer.Text = lastUpdate.LastUpdateString();
        }

        private void Update()
        {
            foreach (var entry in MyccUtil.Rates.CryptoToFiatSourcesWithDetail)
            {
                _views[entry.name].Detail = entry.detail;
                _views[entry.name].ShowIcon = entry.selected;
            }
        }
    }
}

