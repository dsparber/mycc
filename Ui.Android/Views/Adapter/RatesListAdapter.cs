﻿using System.Collections.Generic;
using Android.Content;
using Android.Views;
using Android.Widget;
using MyCC.Ui.DataItems;

namespace MyCC.Ui.Android.Views.Adapter
{
    public class RatesListAdapter : ArrayAdapter<RateItem>
    {
        public RatesListAdapter(Context context, List<RateItem> items) : base(context, 0, items)
        { }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var item = GetItem(position);

            if (convertView == null)
            {
                convertView = LayoutInflater.From(Context).Inflate(Resource.Layout.item_rate, parent, false);
            }

            convertView.FindViewById<TextView>(Resource.Id.text_currency).Text = item.CurrencyCode;
            convertView.FindViewById<TextView>(Resource.Id.text_reference).Text = item.FormattedValue;

            return convertView;
        }
    }
}