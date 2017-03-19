using System.Threading.Tasks;
using MyCC.Core.Settings;
using MyCC.Forms.Resources;
using MyCC.Forms.View.Container;
using Plugin.Fingerprint;
using Plugin.Fingerprint.Abstractions;
using Xamarin.Forms;
using MyCC.Forms.Constants;
using MyCC.Forms.Messages;

namespace MyCC.Forms.View.Pages
{
    public partial class PasswordView
    {
        private readonly bool _pushMainView;
        private readonly bool _goesToBckground;
        private bool _fingerprintCanceled;

        private PasswordView(bool background)
        {
            InitializeComponent();
            _goesToBckground = background;

            PinFrame.OutlineColor = Device.OS == TargetPlatform.Android ? AppConstants.TableBackgroundColor : AppConstants.BorderColor;

            var recognizer = new TapGestureRecognizer();
            recognizer.Tapped += async (sender, e) =>
            {
                _fingerprintCanceled = false;
                PasswordEntry.Unfocus();
                ShowFingerprintIcon.Focus();
                await Authenticate();
            };
            ShowFingerprintIcon.GestureRecognizers.Add(recognizer);

            Messaging.DarkStatusBar.Send(true);
        }

        public PasswordView(bool pushMainView, bool background = false) : this(background)
        {
            _pushMainView = pushMainView;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            if (!_goesToBckground)
            {
                await Authenticate();
            }
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);

            IconView.IsVisible = height >= 350;
        }

        public async Task Authenticate()
        {
            if (ApplicationSettings.IsFingerprintEnabled && !_fingerprintCanceled)
            {
                ShowFingerprintIcon.IsVisible = true;

                var result = await CrossFingerprint.Current.AuthenticateAsync(I18N.UnlockApplication);
                if (result.Authenticated)
                {
                    await Disappear();
                }
                if (result.Status.Equals(FingerprintAuthenticationResultStatus.Canceled))
                {
                    _fingerprintCanceled = true;
                }
            }
            else
            {
                PasswordEntry.Focus();
            }

        }

        private async void PinTextChanged(object sender, TextChangedEventArgs e)
        {
            if (e.NewTextValue.Length > 0)
            {
                PinFrame.OutlineColor = Device.OS == TargetPlatform.Android ? AppConstants.TableBackgroundColor : AppConstants.BorderColor;
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
            if (_pushMainView)
            {
                await Navigation.PushModalAsync(Device.OS == TargetPlatform.Android ? new MasterDetailContainer() as Page : new TabContainerView());
                Messaging.DarkStatusBar.Send(false);
            }
            else
            {
                Messaging.DarkStatusBar.Send(false);
                await Navigation.PopModalAsync();
            }
        }
    }
}
