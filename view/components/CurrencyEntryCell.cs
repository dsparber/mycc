using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using data.storage;
using models;
using MyCryptos.resources;
using Xamarin.Forms;

namespace view.components
{
	public class CurrencyEntryCell : ViewCell
	{
		readonly Label TitleLabel;
		readonly Label SelectedCurrencyLabel;
		readonly Entry AmountEntry;

		readonly INavigation Navigation;

		Currency selectedCurrency;
		bool isAmountEnabled;

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
			set { selectedCurrency = value; if (selectedCurrency != null) SelectedCurrencyLabel.Text = selectedCurrency.Code; }
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

			TitleLabel = new Label();
			TitleLabel.VerticalOptions = LayoutOptions.CenterAndExpand; ;
			TitleLabel.WidthRequest = 100;

			if (IsAmountEnabled)
			{
				TitleLabel.Text = InternationalisationResources.Value;
			}
			else {
				TitleLabel.Text = InternationalisationResources.Currency;
			}

			SelectedCurrencyLabel = new Label();
			SelectedCurrencyLabel.VerticalOptions = LayoutOptions.CenterAndExpand;
			SelectedCurrencyLabel.HorizontalOptions = IsAmountEnabled ? LayoutOptions.End : LayoutOptions.EndAndExpand;

			AmountEntry = new Entry();
			AmountEntry.IsVisible = IsAmountEnabled;
			AmountEntry.HorizontalOptions = LayoutOptions.FillAndExpand;
			AmountEntry.VerticalOptions = LayoutOptions.CenterAndExpand;
			AmountEntry.Keyboard = Keyboard.Numeric;
			AmountEntry.Placeholder = InternationalisationResources.Value;
			AmountEntry.TextChanged += (sender, e) =>
			{
				OnTyped(SelectedMoney);
			};

			var icon = new Image { HeightRequest = 20, Source = ImageSource.FromFile("more.png") };
			icon.HorizontalOptions = LayoutOptions.End;

			var horizontalStack = new StackLayout { Orientation = StackOrientation.Horizontal };
			horizontalStack.Children.Add(TitleLabel);
			horizontalStack.Children.Add(AmountEntry);
			horizontalStack.Children.Add(SelectedCurrencyLabel);
			horizontalStack.Children.Add(icon);
			horizontalStack.VerticalOptions = LayoutOptions.CenterAndExpand;

			var contentView = new ContentView();
			contentView.Margin = new Thickness(15, 0);
			contentView.Content = horizontalStack;

			View = contentView;
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
			readonly ActivityIndicator activityIndicator;
			readonly SearchBar searchBar;
			readonly TableView currenciesTableView;
			readonly CurrencyEntryCell parent;

			Task<List<Currency>> currencies;

			public CurrencyOverlay(CurrencyEntryCell parent)
			{
				this.parent = parent;
				currencies = CurrencyStorage.Instance.AllElements();

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

				activityIndicator = new ActivityIndicator();
				activityIndicator.IsRunning = true;
				activityIndicator.Margin = new Thickness(10);

				currenciesTableView = new TableView();
				currenciesTableView.IsVisible = false;

				var stack = new StackLayout();
				stack.Children.Add(searchBar);
				stack.Children.Add(activityIndicator);
				stack.Children.Add(currenciesTableView);

				Content = stack;
			}

			protected async override void OnAppearing()
			{
				base.OnAppearing();

				var section = new TableSection();

				var currenciesSorted = (await currencies).Distinct().OrderBy(c => c.Code);
				setTableContent(section, currenciesSorted);

				searchBar.TextChanged += (sender, e) =>
				{
					var txt = e.NewTextValue;
					var filtered = currenciesSorted.Where(c => c.Code.ToLower().Contains(txt.ToLower()) || c.Name.ToLower().Contains(txt.ToLower()));
					setTableContent(section, filtered);
				};

				currenciesTableView.Root.Add(section);

				activityIndicator.IsRunning = false;
				activityIndicator.IsVisible = false;
				currenciesTableView.IsVisible = true;

				searchBar.Focus();
			}

			void setTableContent(TableSection section, IEnumerable<Currency> currenciesSorted)
			{
				section.Clear();
				foreach (var c in currenciesSorted)
				{
					var cell = new TextCell { Text = c.Code, Detail = c.Name };
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


