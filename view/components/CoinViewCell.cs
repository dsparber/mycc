using models;
using MyCryptos.resources;
using Xamarin.Forms;

namespace view.components
{
	public class CoinViewCell : ViewCell
	{
		readonly Label SumMoneyLabel;
		readonly Label ReferenceValueLabel;

		Money sumMoney;
		Money referenceValue;

		public Money SumMoney
		{
			get
			{
				return sumMoney;
			}
			set
			{
				sumMoney = value;
				SumMoneyLabel.Text = sumMoney.ToString();
			}
		}
		public Money ReferenceValue
		{
			get
			{
				return referenceValue;
			}
			set
			{
				referenceValue = value;
				ReferenceValueLabel.Text = referenceValue.ToString();
			}
		}

		public bool IsLoading
		{
			set
			{
				if (value)
				{
					ReferenceValueLabel.Text = InternationalisationResources.RefreshingDots;
				}
				else {
					ReferenceValueLabel.Text = referenceValue != null ? referenceValue.ToString() : InternationalisationResources.NoExchangeRateFound;
				}
			}
		}

		public CoinViewCell()
		{
			SumMoneyLabel = new Label();
			ReferenceValueLabel = new Label();

			ReferenceValueLabel.TextColor = Color.Gray;
			ReferenceValueLabel.FontSize = SumMoneyLabel.FontSize * 0.8;

			var stack = new StackLayout();
			stack.Spacing = 0;
			stack.Children.Add(SumMoneyLabel);
			stack.Children.Add(ReferenceValueLabel);

			var contentView = new ContentView();
			contentView.Margin = new Thickness(15, 3);
			contentView.Content = stack;

			View = contentView;
		}
	}
}


