using System.Threading.Tasks;
using MyCC.Core.Settings;
using MyCC.Forms.Resources;
using Plugin.Fingerprint;
using Xamarin.Forms;

namespace MyCC.Forms.view.pages
{
    public partial class PasswordView
    {
        private readonly Page _nextStartupPage;
        private readonly bool _goesToBckground;

        public PasswordView(bool background = false)
        {
            InitializeComponent();
            _goesToBckground = background;
        }

        public PasswordView(Page page, bool background = false) : this(background)
        {
            _nextStartupPage = page;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            if (!_goesToBckground)
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

            if (e.NewTextValue?.Length != ApplicationSettings.PinLength) return;

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

        private async Task Disappear()
        {
            if (_nextStartupPage != null)
            {
                await Navigation.PushModalAsync(_nextStartupPage);
            }
            else
            {
                await Navigation.PopModalAsync();
            }
        }
    }
}
