using System;
using System.Collections.Generic;
using Xamarin.Forms;
using XLabs.Forms.Controls;
using XLabs.Ioc;
using XLabs.Serialization;
using XLabs.Serialization.JsonNET;

namespace MyCryptos.view.components
{
	public class SegmentedControl : ContentView
	{
		public Action<int> SelectionChanged;

		List<string> tabs;
		Color color;
		Color backgroundColor;

		public List<string> Tabs
		{
			get { return tabs; }
			set { tabs = value; updateView(); }
		}

		public Color Color
		{
			get { return color; }
			set { color = value; updateColor(); }
		}

		public Color BgColor
		{
			get { return backgroundColor; }
			set { backgroundColor = value; updateColor(); }
		}

		readonly HybridWebView WebView;

		public SegmentedControl()
		{
			var resolverContainer = new SimpleContainer();

			resolverContainer.Register<IJsonSerializer, JsonSerializer>();

			WebView = new HybridWebView
			{
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand,
			};
			WebView.RegisterCallback("selectedCallback", t =>
			{
				SelectionChanged(Convert.ToInt32(t));
			});

			Content = WebView;
			HeightRequest = 120;

			tabs = new List<string>();
			color = Color.Gray;
			backgroundColor = Color.White;

			updateView();
		}

		public void OnAppearing()
		{
			WebView.LoadFromContent("Html/segmentedControl.html");
			updateView();
			updateColor();
		}

		void updateView()
		{
			WebView.CallJsFunction("setTabs", tabs.ToArray());
		}

		void updateColor()
		{
			WebView.CallJsFunction("setColor", color.ToString(), backgroundColor.ToString());
		}
	}
}
