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

		readonly INavigation Navigation;

		Currency selectedCurrency;

		public Action<Currency> onSelected;
		public Action<Currency> OnSelected
		{
			get { return onSelected ?? ((c) => { }); }
			set { onSelected = value; }
		}

		public Currency SelectedCurrency
		{
			set { selectedCurrency = value; SelectedCurrencyLabel.Text = selectedCurrency.Code; }
			get { return selectedCurrency; }
		}

		public CurrencyEntryCell(INavigation navigation)
		{
			Navigation = navigation;

			TitleLabel = new Label { Text = InternationalisationResources.Currency };
			SelectedCurrencyLabel = new Label();
			SelectedCurrencyLabel.HorizontalOptions = LayoutOptions.EndAndExpand;

			var icon = new Image { HeightRequest = 20, Source = ImageSource.FromFile("more.png") };
			icon.HorizontalOptions = LayoutOptions.End;

			var horizontalStack = new StackLayout { Orientation = StackOrientation.Horizontal };
			horizontalStack.Children.Add(TitleLabel);
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
				Navigation.PushAsync(new CurrencyOverlay(this));
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
				// TODO Implement search

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

				foreach (var c in (await currencies).OrderBy(c => c.Code))
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

				currenciesTableView.Root.Add(section);

				activityIndicator.IsRunning = false;
				activityIndicator.IsVisible = false;
				currenciesTableView.IsVisible = true;
			}
		}
	}
}


