using System;
using System.Collections.Generic;
using System.Linq;
using data.storage;
using MyCryptos.models;
using MyCryptos.resources;
using Xamarin.Forms;
using constants;

namespace MyCryptos.view.components
{
	public class CurrencyEntryCell : ViewCell
	{
		readonly Label TitleLabel;
		readonly Label SelectedCurrencyLabel;
		readonly Entry AmountEntry;

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

			var icon = new Image { HeightRequest = 20, Source = ImageSource.FromFile("more.png") };
			icon.HorizontalOptions = LayoutOptions.End;

			var horizontalStack = new StackLayout { Orientation = StackOrientation.Horizontal };
			horizontalStack.Children.Add(TitleLabel);
			horizontalStack.Children.Add(AmountEntry);
			horizontalStack.Children.Add(SelectedCurrencyLabel);
			horizontalStack.Children.Add(icon);
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
			if (View != null)
			{
				View.GestureRecognizers.Clear();
				View.GestureRecognizers.Add(gestureRecognizer);
			}
		}

		class CurrencyOverlay : ContentPage
		{
			readonly SearchBar searchBar;
			readonly TableView currenciesTableView;
			readonly CurrencyEntryCell parent;

			List<Currency> currencies;

			public CurrencyOverlay(CurrencyEntryCell parent)
			{
				this.parent = parent;

				var type = parent.CurrencyRepositoryType;

				var repos = (type != null) ? CurrencyStorage.Instance.RepositoriesOfType(type) : CurrencyStorage.Instance.Repositories;
				var allElements = CurrencyStorage.Instance.AllElements;

				if (type != null)
				{
					var ids = repos.Select(e => e.Id);
					var currencyMapCodes = CurrencyRepositoryMapStorage.Instance.AllElements.Where(e => ids.Contains(e.RepositoryId)).Select(e => e.Code);
					currencies = allElements.Where(e => currencyMapCodes.Contains(e.Code)).ToList();
				}
				else {
					currencies = allElements;
				}

				Title = InternationalisationResources.Currency;

				EventHandler doneAction = (sender, e) =>
				{
					parent.OnSelected(parent.SelectedCurrency);
					Navigation.PopAsync();
				};

				var done = new ToolbarItem { Text = InternationalisationResources.Save };
				done.Clicked += doneAction;
				ToolbarItems.Add(done);

				searchBar = new SearchBar { Placeholder = InternationalisationResources.SearchCurrencies };
                if (Device.OS == TargetPlatform.Android)
                {
                    searchBar.TextColor = AppConstants.FontColor;
                    searchBar.PlaceholderColor = AppConstants.FontColorLight;
                    searchBar.HeightRequest = searchBar.HeightRequest + 50;
                    searchBar.Margin = new Thickness(0, 0, 0, -51);
                }

				currenciesTableView = new TableView();

				var stack = new StackLayout();
				stack.Children.Add(searchBar);
				stack.Children.Add(currenciesTableView);

				Content = stack;

				var section = new TableSection();

				var currenciesSorted = currencies.Distinct().OrderBy(c => c.Code);
				setTableContent(section, currenciesSorted);

				searchBar.TextChanged += (sender, e) =>
				{
					var txt = e.NewTextValue;
					var filtered = currenciesSorted.Where(c => c.Code.ToLower().Contains(txt.ToLower()) || c.Name.ToLower().Contains(txt.ToLower()));
					setTableContent(section, filtered);
				};

				currenciesTableView.Root.Add(section);

				currenciesTableView.IsVisible = true;
			}

			protected override void OnAppearing()
			{
				base.OnAppearing();

				searchBar.Focus();
			}

			void setTableContent(TableSection section, IEnumerable<Currency> currenciesSorted)
			{
				section.Clear();
				foreach (var c in currenciesSorted)
				{
					var cell = new CustomViewCell { Text = c.Code, Detail = c.Name };
					cell.Tapped += (sender, e) =>
					{
						parent.SelectedCurrency = c;
						parent.OnSelected(parent.SelectedCurrency);
						Navigation.PopAsync();
					};
					section.Add(cell);
				}
			}
		}
	}
}


