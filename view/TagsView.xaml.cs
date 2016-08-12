using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace view
{
	public partial class TagsView : ContentPage
	{
		public TagsView()
		{
			InitializeComponent();
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();
			if (Device.OS == TargetPlatform.Android)
			{
				Title = MyCryptos.resources.Resources.AppName;
			}
		}
	}
}

