using System;
using MyCryptos.models;
using MyCryptos.resources;
using Xamarin.Forms;
using constants;
using MyCryptos.view.overlays;

namespace MyCryptos.view.components
{
    public class CurrencyEntryCell : ViewCell
    {
        private readonly Label titleLabel;
        private readonly Label selectedCurrencyLabel;
        private readonly Entry amountEntry;
        private readonly Image icon;

        private readonly INavigation navigation;

        private Currency selectedCurrency;
        private bool isAmountEnabled;
        private bool isFormRepresentation;

        public Type CurrencyRepositoryType;

        private Action<Currency> onSelected;
        public Action<Currency> OnSelected
        {
            get { return onSelected ?? ((c) => { }); }
            set { onSelected = value; }
        }

        private Action<Money> onTyped;
        public Action<Money> OnTyped
        {
            private get { return onTyped ?? ((c) => { }); }
            set { onTyped = value; }
        }

        public Currency SelectedCurrency
        {
            set
            {
                selectedCurrency = value;

                selectedCurrencyLabel.Text = (selectedCurrency != null) ? selectedCurrency.Code : I18N.SelectCurrency;
                selectedCurrencyLabel.TextColor = (selectedCurrency != null) ? AppConstants.FontColor : AppConstants.FontColorLight;
            }
            get { return selectedCurrency; }
        }

        public Money SelectedMoney
        {
            set { SelectedCurrency = value.Currency; SelectedAmount = value.Amount; }
            get { return new Money(SelectedAmount, SelectedCurrency); }
        }


        private decimal SelectedAmount
        {
            set
            {
                if (value != 0)
                {
                    amountEntry.Text = value.ToString();
                }
            }
            get
            {
                if (amountEntry == null)
                {
                    return 0;
                }
                var txt = amountEntry.Text;
                var selectedAmount = (txt ?? "0").Trim();

                return selectedAmount.Equals(string.Empty) ? 0 : decimal.Parse(selectedAmount);
            }
        }

        public bool IsAmountEnabled
        {
            set
            {
                isAmountEnabled = value;
                amountEntry.IsVisible = value;
                selectedCurrencyLabel.HorizontalOptions = value ? LayoutOptions.End : LayoutOptions.EndAndExpand;
                titleLabel.Text = value ? I18N.Value : I18N.Currency;
            }
            private get { return isAmountEnabled; }
        }

        public bool IsFormRepresentation
        {
            set
            {
                isFormRepresentation = value;
                selectedCurrencyLabel.HorizontalOptions = (IsAmountEnabled) ? LayoutOptions.End : (value) ? LayoutOptions.FillAndExpand : LayoutOptions.EndAndExpand;
            }
            private get { return isFormRepresentation; }
        }

        public bool IsEditable
        {
            set
            {
                amountEntry.IsEnabled = value;
                amountEntry.Opacity = value ? 1 : 0.5;
                selectedCurrencyLabel.Opacity = value ? 1 : 0.5;
            }
            private get { return amountEntry.IsEnabled; }
        }

        public void Unfocus()
        {
            amountEntry.Unfocus();
        }

        public CurrencyEntryCell(INavigation navigation)
        {
            this.navigation = navigation;

            titleLabel = new Label
            {
                TextColor = AppConstants.FontColor,
                WidthRequest = AppConstants.LabelWidth,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                LineBreakMode = LineBreakMode.NoWrap,
                Text = (IsAmountEnabled) ? I18N.Value : I18N.Currency
            };

            selectedCurrencyLabel = new Label
            {
                TextColor = AppConstants.FontColor,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                LineBreakMode = LineBreakMode.NoWrap,
                HorizontalOptions = (IsAmountEnabled) ? LayoutOptions.End : (IsFormRepresentation) ? LayoutOptions.FillAndExpand : LayoutOptions.EndAndExpand,
                Text = (selectedCurrency != null) ? selectedCurrency.Code : I18N.SelectCurrency
            };
            selectedCurrencyLabel.TextColor = (selectedCurrency != null) ? AppConstants.FontColor : AppConstants.FontColorLight;

            amountEntry = new Entry { IsVisible = IsAmountEnabled, HorizontalOptions = LayoutOptions.FillAndExpand, VerticalOptions = LayoutOptions.CenterAndExpand, Keyboard = Keyboard.Numeric, Placeholder = I18N.Value };
            amountEntry.TextChanged += (sender, e) => OnTyped(SelectedMoney);

            if (Device.OS == TargetPlatform.Android)
            {
                titleLabel.FontSize = AppConstants.AndroidFontSize;
                selectedCurrencyLabel.FontSize = AppConstants.AndroidFontSize;
                amountEntry.FontSize = AppConstants.AndroidFontSize;
            }

            icon = new Image
            {
                HeightRequest = 20,
                Source = ImageSource.FromFile("more.png"),
                HorizontalOptions = LayoutOptions.End
            };

            var horizontalStack = new StackLayout { Orientation = StackOrientation.Horizontal };
            horizontalStack.Children.Add(titleLabel);
            horizontalStack.Children.Add(amountEntry);
            horizontalStack.Children.Add(selectedCurrencyLabel);
            horizontalStack.Children.Add(icon);
            horizontalStack.VerticalOptions = LayoutOptions.CenterAndExpand;

            var contentView = new ContentView
            {
                Padding = new Thickness(15, 0),
                Content = horizontalStack,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand
            };
            if (Device.OS == TargetPlatform.Android)
            {
                contentView.BackgroundColor = Color.White;
                View = new ContentView { Content = contentView, BackgroundColor = Color.FromHex("c7d7d4") };
            }
            else
            {
                View = contentView;
            }
            SetTapRecognizer();
        }

        private void SetTapRecognizer()
        {
            var gestureRecognizer = new TapGestureRecognizer();
            gestureRecognizer.Tapped += (sender, e) =>
            {
                if (IsEditable)
                {
                    navigation.PushAsync(new CurrencyOverlay(this));
                }
            };
            if (selectedCurrencyLabel != null)
            {
                selectedCurrencyLabel.GestureRecognizers.Clear();
                selectedCurrencyLabel.GestureRecognizers.Add(gestureRecognizer);
            }
            if (icon != null)
            {
                icon.GestureRecognizers.Clear();
                icon.GestureRecognizers.Add(gestureRecognizer);
            }
        }
    }
}


