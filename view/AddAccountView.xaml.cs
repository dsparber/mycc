using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace view
{
	public partial class AddAccountView : ContentPage
	{
		public AddAccountView()
		{
			InitializeComponent();
		}

		public void Cancel(object sender, EventArgs e)
		{
			Navigation.PopModalAsync();
		}
	}
}

