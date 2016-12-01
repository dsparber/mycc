using System.Threading.Tasks;
using MyCryptos.Core.Settings;
using MyCryptos.Forms.Resources;
using Plugin.Fingerprint;
using Xamarin.Forms;

namespace MyCryptos
{
    public partial class PasswordView : ContentPage
    {
        private Page nextPage;
        private bool goesToBckground;

        public PasswordView(bool background = false)
        {
            InitializeComponent();
            goesToBckground = background;
        }

        public PasswordView(Page page, bool background = false) : this(background)
        {
            nextPage = page;
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();

            if (!goesToBckground)
            {
                await Authenticate();
            }
        }

        public async Task Authenticate()
        {
            if (ApplicationSettings.IsFingerprintEnabled)
            {
                var result = await CrossFingerprint.Current.AuthenticateAsync(I18N.UnlockTheApplication);
                if (result.Authenticated)
                {
                    await Disappear();
                    return;
                }
            }
            PasswordEntry.Focus();
        }

        private async void PinTextChanged(object sender, TextChangedEventArgs e)
        {
            if (e.NewTextValue.Length > 0)
            {
                PinFrame.OutlineColor = Color.White;
            }

            if (e.NewTextValue?.Length == ApplicationSettings.PinLength)
            {
                if (ApplicationSettings.IsPinValid(e.NewTextValue))
                {
                    await Disappear();
                }
                else
                {
                    PasswordEntry.Text = string.Empty;
                    PinFrame.OutlineColor = Color.Red;
                }
            }
        }

        private async Task Disappear()
        {
            if (nextPage != null)
            {
                await Navigation.PushModalAsync(nextPage);
            }
            else
            {
                await Navigation.PopModalAsync();
            }
        }
    }
}
