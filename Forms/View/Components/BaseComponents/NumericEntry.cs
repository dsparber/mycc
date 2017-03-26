using System;
using System.Globalization;
using System.Text.RegularExpressions;
using Xamarin.Forms;

namespace MyCC.Forms.View.Components.BaseComponents
{
    public class NumericEntry : Entry
    {
        public bool IsPin
        {
            set { IsPassword = value; }
            get { return IsPassword; }
        }

        public NumericEntry()
        {
            Keyboard = Keyboard.Numeric;
            TextChanged += (sender, args) =>
            {
                var seperator = CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator;

                var entry = (Entry)sender;
                var val = entry.Text;

                if (val.Length == 0) return;
                if (!IsPin && val.Split(new[] { seperator }, StringSplitOptions.RemoveEmptyEntries).Length <= 2 &&
                     Regex.IsMatch(val.Replace(seperator, string.Empty), @"^\d+$")) return;
                if (IsPin && Regex.IsMatch(val, @"^\d+$")) return;

                val = val.Remove(val.Length - 1);
                entry.Text = val;
            };
        }
    }
}