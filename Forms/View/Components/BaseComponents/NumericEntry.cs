using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Xamarin.Forms;

namespace MyCC.Forms.View.Components.BaseComponents
{
    public class NumericEntry : Entry
    {
        public bool IsPin
        {
            set => IsPassword = value;
            get => IsPassword;
        }

        public NumericEntry()
        {
            Keyboard = Keyboard.Numeric;
            TextChanged += (sender, args) =>
            {
                var seperator = CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator[0];

                var entry = (Entry)sender;
                var val = entry.Text;

                if (val.StartsWith(seperator.ToString()))
                {
                    val = $"0{val}";
                    entry.Text = val;
                }

                if (val.Length == 0) return;
                if (!IsPin && char.IsDigit(val[0]) && (val.Count(x => x == seperator) == 0 || val.Count(x => x == seperator) == 1 &&
                     $"{val}x".Split(new[] { seperator }, StringSplitOptions.RemoveEmptyEntries)[1].Length <= 9) &&
                     Regex.IsMatch(val.Replace(seperator.ToString(), string.Empty), @"^\d+$")) return;
                if (IsPin && Regex.IsMatch(val, @"^\d+$")) return;

                val = val.Remove(val.Length - 1);
                entry.Text = val;
            };
        }
    }
}