using System.Threading.Tasks;
using MyCC.Core.Settings;
using MyCC.Forms.Constants;
using MyCC.Forms.Resources;
using MyCC.Forms.View.Container;
using MyCC.Ui.Messages;
using Plugin.Fingerprint;
using Plugin.Fingerprint.Abstractions;
using Xamarin.Forms;

namespace MyCC.Forms.View.Overlays
{
    public partial class PasswordOverlay
    {
        private readonly bool _pushMainView;
        private readonly bool _goesToBckground;
        private bool _fingerprintCanceled;

        private PasswordOverlay(bool background)
        {
            InitializeComponent();
            _goesToBckground = background;

            PinFrame.OutlineColor = Device.RuntimePlatform.Equals(Device.Android) ? AppConstants.TableBackgroundColor : AppConstants.BorderColor;

            var recognizer = new TapGestureRecognizer();
            recognizer.Tapped += async (sender, e) =>
            {
                _fingerprintCanceled = false;
                PasswordEntry.Unfocus();
                ShowFingerprintIcon.Focus();
                await Authenticate();
            };
            ShowFingerprintIcon.GestureRecognizers.Add(recognizer);

            Messaging.Status.DarkStatusBar.Send(true);
        }

        public PasswordOverlay(bool pushMainView, bool background = false) : this(background)
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

                var result =
                    await CrossFingerprint.Current.AuthenticateAsync(new AuthenticationRequestConfiguration(I18N.UnlockApplication)
                    {
                        CancelTitle = I18N.Cancel,
                        FallbackTitle = I18N.Cancel
                    });
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
                PinFrame.OutlineColor = Device.RuntimePlatform.Equals(Device.Android) ? AppConstants.TableBackgroundColor : AppConstants.BorderColor;
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
            PinFrame.Unfocus();
            if (_pushMainView)
            {
                await Navigation.PushModalAsync(Device.RuntimePlatform.Equals(Device.Android) ? new MasterDetailContainer() as Page : new TabContainerView());
                Messaging.Status.DarkStatusBar.Send(false);
            }
            else
            {
                Messaging.Status.DarkStatusBar.Send(false);
                await Navigation.PopModalAsync();
            }
        }
    }
}
