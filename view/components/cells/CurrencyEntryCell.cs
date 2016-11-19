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
		readonly Label TitleLabel;
		readonly Label SelectedCurrencyLabel;
		readonly Entry AmountEntry;
        readonly Image Icon;

		readonly INavigation Navigation;

		Currency selectedCurrency;
		bool isAmountEnabled;
		bool isFormRepresentation;

		public Type CurrencyRepositoryType;

		public Action<Currency> onSelected;
		public Action<Currency> OnSelected
		{
			get { return onSelected ?? ((c) => { }); }
			set { onSelected = value; }
		}
		public Action<Money> onTyped;
		public Action<Money> OnTyped
		{
			get { return onTyped ?? ((c) => { }); }
			set { onTyped = value; }
		}

		public Currency SelectedCurrency
		{
			set
			{
				selectedCurrency = value;

				SelectedCurrencyLabel.Text = (selectedCurrency != null) ? selectedCurrency.Code : InternationalisationResources.SelectCurrency;
				SelectedCurrencyLabel.TextColor = (selectedCurrency != null) ? AppConstants.FontColor : AppConstants.FontColorLight;
			}
			get { return selectedCurrency; }
		}

		public Money SelectedMoney
		{
			set { SelectedCurrency = value.Currency; SelectedAmount = value.Amount; }
			get { return new Money(SelectedAmount, SelectedCurrency); }
		}


		public decimal SelectedAmount
		{
			set
			{
				if (value != 0)
				{
					AmountEntry.Text = value.ToString();
				}
			}
			get
			{
				if (AmountEntry == null)
				{
					return 0;
				}
				var txt = AmountEntry.Text;
				var selectedAmount = (txt ?? "0");
				if (selectedAmount.Trim().Equals(string.Empty))
				{
					return 0;
				}
				return decimal.Parse(selectedAmount);
			}
		}

		public bool IsAmountEnabled
		{
			set
			{
				isAmountEnabled = value;
				AmountEntry.IsVisible = value;
				SelectedCurrencyLabel.HorizontalOptions = value ? LayoutOptions.End : LayoutOptions.EndAndExpand;
				TitleLabel.Text = value ? InternationalisationResources.Value : InternationalisationResources.Currency;
			}
			get { return isAmountEnabled; }
		}

		public bool IsFormRepresentation
		{
			set
			{
				isFormRepresentation = value;
				SelectedCurrencyLabel.HorizontalOptions = (IsAmountEnabled) ? LayoutOptions.End : (value) ? LayoutOptions.FillAndExpand : LayoutOptions.EndAndExpand;
			}
			get { return isFormRepresentation; }
		}

		public bool IsEditable
		{
			set
			{
				AmountEntry.IsEnabled = value;
				AmountEntry.Opacity = value ? 1 : 0.5;
				SelectedCurrencyLabel.Opacity = value ? 1 : 0.5;
			}
			get { return AmountEntry.IsEnabled; }
		}

		public CurrencyEntryCell(INavigation navigation)
		{
			Navigation = navigation;

			TitleLabel = new Label { TextColor = AppConstants.FontColor, WidthRequest = AppConstants.LabelWidth, VerticalOptions = LayoutOptions.CenterAndExpand };
			TitleLabel.Text = (IsAmountEnabled) ? InternationalisationResources.Value : InternationalisationResources.Currency;

			SelectedCurrencyLabel = new Label { TextColor = AppConstants.FontColor, VerticalOptions = LayoutOptions.CenterAndExpand };
			SelectedCurrencyLabel.HorizontalOptions = (IsAmountEnabled) ? LayoutOptions.End : (IsFormRepresentation) ? LayoutOptions.FillAndExpand : LayoutOptions.EndAndExpand;
			SelectedCurrencyLabel.Text = (selectedCurrency != null) ? selectedCurrency.Code : InternationalisationResources.SelectCurrency;
			SelectedCurrencyLabel.TextColor = (selectedCurrency != null) ? AppConstants.FontColor : AppConstants.FontColorLight;

			AmountEntry = new Entry { IsVisible = IsAmountEnabled, HorizontalOptions = LayoutOptions.FillAndExpand, VerticalOptions = LayoutOptions.CenterAndExpand, Keyboard = Keyboard.Numeric, Placeholder = InternationalisationResources.Value };
			AmountEntry.TextChanged += (sender, e) => OnTyped(SelectedMoney);

			if (Device.OS == TargetPlatform.Android)
			{
				TitleLabel.FontSize = AppConstants.AndroidFontSize;
				SelectedCurrencyLabel.FontSize = AppConstants.AndroidFontSize;
				AmountEntry.FontSize = AppConstants.AndroidFontSize;
			}

			Icon = new Image { HeightRequest = 20, Source = ImageSource.FromFile("more.png") };
			Icon.HorizontalOptions = LayoutOptions.End;

			var horizontalStack = new StackLayout { Orientation = StackOrientation.Horizontal };
			horizontalStack.Children.Add(TitleLabel);
			horizontalStack.Children.Add(AmountEntry);
			horizontalStack.Children.Add(SelectedCurrencyLabel);
			horizontalStack.Children.Add(Icon);
			horizontalStack.VerticalOptions = LayoutOptions.CenterAndExpand;

			var contentView = new ContentView();
			contentView.Padding = new Thickness(15, 0);
			contentView.Content = horizontalStack;
			contentView.HorizontalOptions = LayoutOptions.FillAndExpand;
			contentView.VerticalOptions = LayoutOptions.FillAndExpand;
			if (Device.OS == TargetPlatform.Android)
			{
				contentView.BackgroundColor = Color.White;
				View = new ContentView { Content = contentView, BackgroundColor = Color.FromHex("c7d7d4") };
			}
			else
			{
				View = contentView;
			}
			setTapRecognizer();
		}

		void setTapRecognizer()
		{
			var gestureRecognizer = new TapGestureRecognizer();
			gestureRecognizer.Tapped += (sender, e) =>
			{
				if (IsEditable)
				{
					Navigation.PushAsync(new CurrencyOverlay(this));
				}
			};
			if (SelectedCurrencyLabel != null)
			{
                SelectedCurrencyLabel.GestureRecognizers.Clear();
                SelectedCurrencyLabel.GestureRecognizers.Add(gestureRecognizer);
			}
            if (Icon != null)
            {
                Icon.GestureRecognizers.Clear();
                Icon.GestureRecognizers.Add(gestureRecognizer);
            }
        }
	}
}


