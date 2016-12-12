using System.Threading.Tasks;
using MyCryptos.Core.settings;
using MyCryptos.Forms.Resources;
using Plugin.Fingerprint;
using Xamarin.Forms;

namespace MyCryptos.Forms.view.pages
{
    public partial class PasswordView
    {
        private readonly Page nextStartupPage;
        private readonly bool goesToBckground;

        public PasswordView(bool background = false)
        {
            InitializeComponent();
            goesToBckground = background;
        }

        public PasswordView(Page page, bool background = false) : this(background)
        {
            nextStartupPage = page;
        }

        protected override async void OnAppearing()
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
            if (nextStartupPage != null)
            {
                await Navigation.PushModalAsync(nextStartupPage);
            }
            else
            {
                await Navigation.PopModalAsync();
            }
        }
    }
}
