using MyCryptos.Forms.helpers;
using MyCryptos.Forms.Messages;
using Xamarin.Forms;

namespace MyCryptos.view.components
{
	public partial class HeaderView
	{
		private double defaultSize = 36;
		private double defaultSizeSmall = 24;

		public string TitleText
		{
			private get { return TitleLabel.Text ?? string.Empty; }
			set
			{
				TitleLabel.Text = GetText(value);
				AdaptSize();
			}
		}

		public string TitleTextSmall
		{
			private get { return TitleLabelSmall.Text ?? string.Empty; }
			set
			{
				TitleLabelSmall.Text = GetText(value);
				AdaptSize();
			}
		}

		public string InfoText
		{
			private get { return InfoLabel.Text ?? string.Empty; }
			set
			{
				InfoLabel.Text = GetText(value);
			}
		}

		public string LoadingText
		{
			private get { return RefreshingLabel.Text; }
			set
			{
				RefreshingLabel.Text = GetText(value);
			}
		}

		public bool IsLoading
		{
			get { return LoadingIndicator.IsRunning; }
			set { LoadingPanel.IsVisible = value; LoadingIndicator.IsRunning = value; InfoLabel.IsVisible = !value; }
		}

		public HeaderView(bool subscribeToRefresh) : this()
		{
			if (subscribeToRefresh)
			{
				Messaging.FetchMissingRates.SubscribeStartedAndFinished(this, () => Device.BeginInvokeOnMainThread(() => IsLoading = true), () => Device.BeginInvokeOnMainThread(() => IsLoading = false));
				Messaging.Loading.SubscribeStartedAndFinished(this, () => Device.BeginInvokeOnMainThread(() => IsLoading = true), () => Device.BeginInvokeOnMainThread(() => IsLoading = false));
				Messaging.UpdatingAccounts.SubscribeStartedAndFinished(this, () => Device.BeginInvokeOnMainThread(() => IsLoading = true), () => Device.BeginInvokeOnMainThread(() => IsLoading = false));
				Messaging.UpdatingAccountsAndRates.SubscribeStartedAndFinished(this, () => Device.BeginInvokeOnMainThread(() => IsLoading = true), () => Device.BeginInvokeOnMainThread(() => IsLoading = false));
			}
		}

		public HeaderView()
		{
			InitializeComponent();

			LoadingIndicator.HeightRequest = 18;
			LoadingIndicator.WidthRequest = 18;
			if (Device.OS == TargetPlatform.Android)
			{

				LoadingIndicator.VerticalOptions = LayoutOptions.Center;
			}

			Padding = new Thickness(0, 0, 0, 10);

			TitleText = TitleText;
			InfoText = InfoText;
			LoadingText = LoadingText;

			Messaging.Layout.SubscribeValueChanged(this, AdaptSize);
		}

		private static string GetText(string text)
		{
			text = text?.Trim();
			return string.IsNullOrEmpty(text) ? " " : text;
		}

		void AdaptSize()
		{
			var size = (float?)defaultSize + 0.25f;
			var sizeSmall = (float?)defaultSizeSmall + 0.25f;
			double width = 0, availableWidth = 0;

			do
			{
				size -= 0.25f; sizeSmall -= 0.25f;
				width = DependencyService.Get<ITextSizeHelper>().CalculateWidth(TitleText, size, true).Item2
						+ DependencyService.Get<ITextSizeHelper>().CalculateWidth(TitleTextSmall, sizeSmall, true).Item2;
				availableWidth = Width - 40;

			} while (availableWidth > 0 && width > availableWidth);

			TitleLabel.FontSize = (double)size;
			TitleLabelSmall.FontSize = (double)sizeSmall;
		}
	}
}