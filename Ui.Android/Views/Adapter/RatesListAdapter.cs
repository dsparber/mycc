using System;
using System.Collections.Generic;
using Android.Content;
using Android.Views;
using Android.Widget;
using MyCC.Core.Currencies.Models;
using MyCC.Ui.DataItems;

namespace MyCC.Ui.Android.Views.Adapter
{
    public class RatesListAdapter : ArrayAdapter<RateItem>
    {
        private readonly Dictionary<int, ImageView> _removeIcons;

        private bool _editingEnabled;

        public bool EditingEnabled
        {
            set
            {
                _editingEnabled = value;
                foreach (var ic in _removeIcons.Values)
                {
                    ic.Visibility = value ? ViewStates.Visible : ViewStates.Gone;
                }
            }
        }

        public Action CurrencyRemoved { private get; set; }

        public RatesListAdapter(Context context, List<RateItem> items) : base(context, 0, items)
        {
            _removeIcons = new Dictionary<int, ImageView>();
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var item = GetItem(position);

            if (convertView == null)
            {
                convertView = LayoutInflater.From(Context).Inflate(Resource.Layout.item_rate, parent, false);
            }

            convertView.FindViewById<TextView>(Resource.Id.text_currency).Text = item.CurrencyCode;
            convertView.FindViewById<TextView>(Resource.Id.text_reference).Text = item.FormattedValue;

            var removeIcon = convertView.FindViewById<ImageView>(Resource.Id.image_remove);
            removeIcon.Visibility = _editingEnabled ? ViewStates.Visible : ViewStates.Gone;
            removeIcon.SetOnClickListener(new ClickListener(item.CurrencyId, this));

            if (_removeIcons.ContainsKey(position))
            {
                _removeIcons[position] = removeIcon;
            }
            else
            {
                _removeIcons.Add(position, removeIcon);
            }

            return convertView;
        }

        private class ClickListener : Java.Lang.Object, View.IOnClickListener
        {
            private readonly string _currencyId;
            private readonly RatesListAdapter _adapter;

            public ClickListener(string currencyId, RatesListAdapter adapter)
            {
                _currencyId = currencyId;
                _adapter = adapter;
            }
            public void OnClick(View v)
            {
                UiUtils.Edit.RemoveWatchedCurrency(_currencyId);
                _adapter.CurrencyRemoved?.Invoke();
            }
        }


    }
}