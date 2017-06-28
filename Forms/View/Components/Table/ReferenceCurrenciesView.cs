using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.Serialization;
using MyCC.Forms.Constants;
using MyCC.Forms.View.Components.BaseComponents;
using MyCC.Forms.View.Components.CellViews;
using MyCC.Ui.DataItems;
using Xamarin.Forms;

namespace MyCC.Forms.View.Components.Table
{
    public class ReferenceCurrenciesView : ContentView
    {
        public (IEnumerable<ReferenceValueItem> ReferenceValueItems, List<SortButtonItem> SortButtons) Items { set => Device.BeginInvokeOnMainThread(() => UpdateView(value)); }

        public string Title { set => Device.BeginInvokeOnMainThread(() => _sectionHeader.Title = value); }


        private readonly SectionHeaderView _sectionHeader;
        private readonly HybridWebView _webView;

        private readonly Dictionary<int, Action> _headerClickCallbacks;
        private static int _currentId;
        private bool _loadFinished;

        public ReferenceCurrenciesView()
        {
            _webView = new HybridWebView("Html/equalsTable.html");
            _headerClickCallbacks = new Dictionary<int, Action>();

            _webView.RegisterCallback("CallbackSizeAllocated", sizeString =>
            {
                var size = int.Parse(sizeString);
                Device.BeginInvokeOnMainThread(() => _webView.HeightRequest = size);
            });

            _webView.RegisterCallback("HeaderClickedCallback", id => _headerClickCallbacks[int.Parse(id)].Invoke());


            var stack = new StackLayout { Spacing = 0, HorizontalOptions = LayoutOptions.FillAndExpand, VerticalOptions = LayoutOptions.FillAndExpand, BackgroundColor = AppConstants.TableBackgroundColor };

            _sectionHeader = new SectionHeaderView(false);
            stack.Children.Add(_sectionHeader);
            stack.Children.Add(_webView);

            Content = stack;
        }

        public void UpdateView((IEnumerable<ReferenceValueItem> ReferenceValueItems, List<SortButtonItem> SortButtons) data)
        {
            if (!_loadFinished)
            {
                _webView.LoadFinished = () => { _loadFinished = true; UpdateView(data); };
            }
            else
            {
                var items = data.ReferenceValueItems?.Select(item => new Data(item)).ToList();
                if (items == null || !items.Any()) return;

                Device.BeginInvokeOnMainThread(() =>
                {
                    _headerClickCallbacks.Clear();
                    _currentId = 0;
                    _webView.CallJsFunction("setHeader", data.SortButtons.Select(button => new HeaderData(button, _currentId += 1, this)), string.Empty);
                    _webView.CallJsFunction("updateTable", items.ToArray(), string.Empty);
                });
            }
        }

        [DataContract]
        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
        [SuppressMessage("ReSharper", "NotAccessedField.Global")]
        public class Data
        {
            [DataMember]
            public readonly string Amount;
            [DataMember]
            public readonly string Code;
            [DataMember]
            public readonly string Rate;
            [DataMember]
            public readonly bool Expanded;

            public Data(ReferenceValueItem referenceValue)
            {
                Code = referenceValue.CurrencyCode;
                Amount = referenceValue.FormattedAmount;
                Rate = referenceValue.FormattedRate;
                Expanded = referenceValue.Amount != 1;
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
            public readonly int Id;
            [DataMember]
            public readonly bool? Ascending;

            public HeaderData(SortButtonItem sortButtonItem, int id, ReferenceCurrenciesView parent)
            {
                Text = sortButtonItem.Text;
                Id = id;
                Ascending = sortButtonItem.SortAscending;
                parent._headerClickCallbacks[id] = sortButtonItem.OnClick;

            }
        }
    }
}
