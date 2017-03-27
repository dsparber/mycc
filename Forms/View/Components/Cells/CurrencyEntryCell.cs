using System;
using System.Collections.Generic;
using MyCC.Core.Account.Models.Base;
using MyCC.Core.Currency.Model;
using MyCC.Core.Currency.Storage;
using MyCC.Forms.Constants;
using MyCC.Forms.Resources;
using MyCC.Forms.View.Components.BaseComponents;
using MyCC.Forms.View.Overlays;
using Xamarin.Forms;

namespace MyCC.Forms.View.Components.Cells
{
	public class CurrencyEntryCell : ViewCell
	{
		private readonly Label _titleLabel;
		private readonly Label _selectedCurrencyLabel;
		private readonly Entry _amountEntry;
		private readonly Image _icon;

		private readonly INavigation _navigation;

		private Currency _selectedCurrency;
		private bool _isAmountEnabled;
		private bool _isFormRepresentation;

		public readonly Func<IEnumerable<Currency>> CurrenciesToSelect;

		public Action<Currency> OnSelected;

		private Action<Money> _onTyped;
		public Action<Money> OnTyped
		{
			private get { return _onTyped ?? (c => { }); }
			set { _onTyped = value; }
		}

		public Currency SelectedCurrency
		{
			set
			{
				_selectedCurrency = value;

				_selectedCurrencyLabel.Text = _selectedCurrency != null ? _selectedCurrency.Code : I18N.SelectCurrency;
				_selectedCurrencyLabel.TextColor = _selectedCurrency != null ? AppConstants.FontColor : AppConstants.FontColorLight;
			}
			get { return _selectedCurrency; }
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
					_amountEntry.Text = value.ToString();
				}
			}
			get
			{
				if (_amountEntry == null)
				{
					return 0;
				}
				var txt = _amountEntry.Text;
				var selectedAmount = (txt ?? "0").Trim();

				try
				{
					return selectedAmount.Equals(string.Empty) ? 0 : decimal.Parse(selectedAmount);
				}
				catch
				{
					return 0;
				}
			}
		}

		public bool IsAmountEnabled
		{
			set
			{
				_isAmountEnabled = value;
				_amountEntry.IsVisible = value;
				_selectedCurrencyLabel.HorizontalOptions = value ? LayoutOptions.End : LayoutOptions.EndAndExpand;
				_titleLabel.Text = value ? I18N.Value : I18N.Currency;
			}
			private get { return _isAmountEnabled; }
		}

		public bool IsFormRepresentation
		{
			set
			{
				_isFormRepresentation = value;
				_selectedCurrencyLabel.HorizontalOptions = IsAmountEnabled ? LayoutOptions.End : value ? LayoutOptions.FillAndExpand : LayoutOptions.EndAndExpand;
			}
			private get { return _isFormRepresentation; }
		}

		public bool IsEditable
		{
			set
			{
				_amountEntry.IsEnabled = value;
				_amountEntry.Opacity = value ? 1 : 0.5;
				_selectedCurrencyLabel.Opacity = value ? 1 : 0.5;
			}
			private get { return _amountEntry.IsEnabled; }
		}

		public void Unfocus()
		{
			_amountEntry.Unfocus();
		}

		public CurrencyEntryCell(INavigation navigation, Func<IEnumerable<Currency>> currenciesToSelect)
		{
			_navigation = navigation;
			CurrenciesToSelect = currenciesToSelect;

			_titleLabel = new Label
			{
				TextColor = AppConstants.FontColor,
				WidthRequest = AppConstants.LabelWidth,
				MinimumWidthRequest = AppConstants.LabelWidth,
				VerticalOptions = LayoutOptions.CenterAndExpand,
				LineBreakMode = LineBreakMode.NoWrap,
				Text = IsAmountEnabled ? I18N.Value : I18N.Currency
			};

			_selectedCurrencyLabel = new Label
			{
				TextColor = AppConstants.FontColor,
				VerticalOptions = LayoutOptions.CenterAndExpand,
				LineBreakMode = LineBreakMode.NoWrap,
				HorizontalOptions = IsAmountEnabled ? LayoutOptions.End : IsFormRepresentation ? LayoutOptions.FillAndExpand : LayoutOptions.EndAndExpand,
				Text = _selectedCurrency != null ? _selectedCurrency.Code : I18N.SelectCurrency
			};
			_selectedCurrencyLabel.TextColor = _selectedCurrency != null ? AppConstants.FontColor : AppConstants.FontColorLight;

			_amountEntry = new NumericEntry { IsVisible = IsAmountEnabled, HorizontalOptions = LayoutOptions.FillAndExpand, VerticalOptions = LayoutOptions.CenterAndExpand, Placeholder = I18N.Value };
			_amountEntry.TextChanged += (sender, e) => OnTyped(SelectedMoney);

			if (Device.OS == TargetPlatform.Android)
			{
				_titleLabel.FontSize = AppConstants.AndroidFontSize;
				_selectedCurrencyLabel.FontSize = AppConstants.AndroidFontSize;
				_amountEntry.FontSize = AppConstants.AndroidFontSize;
			}

			_icon = new Image
			{
				HeightRequest = 20,
				Source = ImageSource.FromFile("more.png"),
				HorizontalOptions = LayoutOptions.End
			};

			var horizontalStack = new StackLayout { Orientation = StackOrientation.Horizontal };
			horizontalStack.Children.Add(_titleLabel);
			horizontalStack.Children.Add(_amountEntry);
			horizontalStack.Children.Add(_selectedCurrencyLabel);
			horizontalStack.Children.Add(_icon);
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
			Height = 44;
			SetTapRecognizer();
		}

		private void SetTapRecognizer()
		{
			var gestureRecognizer = new TapGestureRecognizer();
			gestureRecognizer.Tapped += (sender, e) =>
			{
				if (IsEditable)
				{
					var allCurrencies = new Func<IEnumerable<Currency>>(() => CurrencyStorage.Instance.AllElements);

					_navigation.PushAsync(new CurrencyOverlay(CurrenciesToSelect ?? allCurrencies, I18N.Currency)
					{
						CurrencySelected = c =>
						{
							SelectedCurrency = c;
							OnSelected?.Invoke(c);
						}
					});
				}
			};
			if (_selectedCurrencyLabel != null)
			{
				_selectedCurrencyLabel.GestureRecognizers.Clear();
				_selectedCurrencyLabel.GestureRecognizers.Add(gestureRecognizer);
			}
			if (_icon == null) return;

			_icon.GestureRecognizers.Clear();
			_icon.GestureRecognizers.Add(gestureRecognizer);
		}
	}
}


