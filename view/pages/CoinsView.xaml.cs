using System;
using System.Linq;
using data.settings;
using enums;
using message;
using MyCryptos.resources;
using Xamarin.Forms;
using MyCryptos.helpers;
using System.Collections.Generic;
using tasks;
using MyCryptos.view;
using MyCryptos.view.components;

namespace view
{
	public partial class CoinsView : ContentPage
	{
		ContentView TableView;
		CoinsGraphView GraphView;

		bool loadedView;

		public CoinsView()
		{
			InitializeComponent();

			TableView = new CoinsTableView();
			GraphView = new CoinsGraphView(Navigation)
			{
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand
			};

			Stack.Children.Add(TableView);
			Stack.Children.Add(GraphView);
			GraphView.IsVisible = false;

			Tabs.Tabs = new List<string> { I18N.Table, I18N.Graph };

			addSubscriber();

			if (Device.OS == TargetPlatform.Android)
			{
				Title = string.Empty;
			}

			Tabs.SelectionChanged = selected =>
			{
				TableView.IsVisible = (selected == 0);
				GraphView.IsVisible = (selected == 1);
			};

			var pages = new List<int>();
			var i = 0;
			foreach (var c in ApplicationSettings.ReferenceCurrencies)
			{
				pages.Add(i);
				i += 1;
			}

			HeaderCarousel.ItemsSource = pages;
			HeaderCarousel.Position = ApplicationSettings.ReferenceCurrencies.IndexOf(ApplicationSettings.BaseCurrency);
			HeaderCarousel.ItemTemplate = new HeaderTemplateSelector();
			HeaderCarousel.PositionSelected += PositionSelected;
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();
			if (!loadedView)
			{
				loadedView = true;
				GraphView.OnAppearing();
			}
		}


		public void PositionSelected(object sender, EventArgs e)
		{
			var currencies = ApplicationSettings.ReferenceCurrencies;
			if (HeaderCarousel.Position > 0 && HeaderCarousel.Position < ApplicationSettings.ReferenceCurrencies.Count)
			{
				ApplicationSettings.BaseCurrency = currencies[HeaderCarousel.Position];
			}
		}

		void SetHeaderCarousel()
		{
			while (HeaderCarousel.ItemsSource.Count > 0)
			{
				HeaderCarousel.RemovePage(0);
			}

			var x = 0;
			foreach (var c in ApplicationSettings.ReferenceCurrencies.ToList())
			{
				HeaderCarousel.InsertPage(x, HeaderCarousel.ItemsSource.Count - 1);
				x += 1;
			}

			HeaderCarousel.ItemTemplate = new HeaderTemplateSelector();
			HeaderCarousel.Position = ApplicationSettings.ReferenceCurrencies.IndexOf(ApplicationSettings.BaseCurrency);
		}

		public async void Add(object sender, EventArgs e)
		{
			await Navigation.PushOrPushModal(new AddSourceView());
		}

		void addSubscriber()
		{
			MessagingCenter.Subscribe<FetchSpeed>(this, MessageConstants.StartedFetching, speed => setLoadingAnimation(speed, true));
			MessagingCenter.Subscribe<FetchSpeed>(this, MessageConstants.DoneFetching, speed => setLoadingAnimation(speed, false));

			MessagingCenter.Subscribe<string>(this, MessageConstants.UpdatedReferenceCurrency, s => HeaderCarousel.Position = ApplicationSettings.ReferenceCurrencies.IndexOf(ApplicationSettings.BaseCurrency));
			MessagingCenter.Subscribe<string>(this, MessageConstants.UpdatedReferenceCurrencies, s => SetHeaderCarousel());
		}

		void setLoadingAnimation(FetchSpeed speed, bool loading)
		{
			if (loading)
			{
				IsBusy = loading;
			}
			else
			{
				if (speed.Speed != FetchSpeedEnum.FAST)
				{
					IsBusy = false;
				}
			}
		}

		void Refresh(object sender, EventArgs e)
		{
			AppTasks.Instance.StartFetchTask(false);
		}

		private class HeaderTemplateSelector : DataTemplateSelector
		{
			private readonly List<DataTemplate> templates;

			public HeaderTemplateSelector()
			{
				templates = ApplicationSettings.ReferenceCurrencies.Select(e => new DataTemplate(() => new CoinsHeaderView(e))).ToList();
			}

			protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
			{
				return templates[(int)item];
			}
		}
	}
}