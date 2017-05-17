using System.Collections.Generic;
using Android.Content;
using Android.Graphics;
using Android.Support.V4.Content;
using Android.Views;
using Android.Widget;
using MyCC.Core.Settings;
using MyCC.Core.Types;
using MyCC.Ui.Android.Helpers;
using MyCC.Ui.DataItems;

namespace MyCC.Ui.Android.Views.Adapter
{
    public class AssetsListAdapter : ArrayAdapter<AssetItem>
    {
        private readonly Context _context;

        public AssetsListAdapter(Context context, IList<AssetItem> items) : base(context, 0, items)
        {
            _context = context;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var item = GetItem(position);

            if (convertView == null)
            {
                convertView = LayoutInflater.From(Context).Inflate(Resource.Layout.item_asset, parent, false);
            }


            var currency = convertView.FindViewById<TextView>(Resource.Id.text_currency);
            currency.Text = item.CurrencyCode;

            var amount = convertView.FindViewById<TextView>(Resource.Id.text_amount);
            amount.Text = item.FormattedValue;
            amount.Visibility = convertView.Width < 480.DpToPx() && ApplicationSettings.AssetsColumToHideIfSmall == ColumnToHide.Amount ? ViewStates.Gone : ViewStates.Visible;


            var reference = convertView.FindViewById<TextView>(Resource.Id.text_reference);
            reference.Text = item.FormattedReferenceValue;
            reference.Visibility = convertView.Width < 480.DpToPx() && ApplicationSettings.AssetsColumToHideIfSmall == ColumnToHide.Value ? ViewStates.Gone : ViewStates.Visible;

            if (!item.Enabled)
            {
                var disabledColor = new Color(ContextCompat.GetColor(_context, Resource.Color.colorDisabled));

                reference.SetTextColor(disabledColor);
                currency.SetTextColor(disabledColor);
                amount.SetTextColor(disabledColor);
            }

            convertView.ViewTreeObserver.GlobalLayout += (sender, args) =>
            {
                amount.Visibility = convertView.Width < 480.DpToPx() && ApplicationSettings.AssetsColumToHideIfSmall == ColumnToHide.Amount ? ViewStates.Gone : ViewStates.Visible;
                reference.Visibility = convertView.Width < 480.DpToPx() && ApplicationSettings.AssetsColumToHideIfSmall == ColumnToHide.Value ? ViewStates.Gone : ViewStates.Visible;
            };


            return convertView;
        }
    }
}