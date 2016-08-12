using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace view
{
	public partial class AddCoinView : ContentPage
	{
		public AddCoinView()
		{
			InitializeComponent();
		}

		public void Cancel(object sender, EventArgs e)
		{
			Navigation.PopModalAsync();
		}
	}
}

