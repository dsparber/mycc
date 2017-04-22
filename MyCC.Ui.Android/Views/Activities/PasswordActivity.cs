using System;
using System.Threading.Tasks;
using Android.App;
using Android.OS;
using Android.Security.Keystore;
using Android.Support.V4.Hardware.Fingerprint;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using Java.Lang;
using Javax.Crypto;
using MyCC.Core.Settings;
using MyCC.Ui.Android.Views.Dialogs;
using CancellationSignal = Android.Support.V4.OS.CancellationSignal;


namespace MyCC.Ui.Android.Views.Activities
{
    [Activity(Theme = "@style/LockscreenTheme", NoHistory = true)]
    public class PasswordActivity : MyccActivity
    {
        private TextView _editPin;
        private ImageView _fingerprintIcon;
        private CancellationSignal _cancellationSignal;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_password);

            _editPin = FindViewById<TextView>(Resource.Id.edit_pin);
            _fingerprintIcon = FindViewById<ImageView>(Resource.Id.image_fingerprint);

            var fingerprintManager = FingerprintManagerCompat.From(this);
            var keyguardManager = (KeyguardManager)GetSystemService(KeyguardService);

            _fingerprintIcon.Click += (sender, args) => AuthenticateWithFingerprint();

            if (fingerprintManager.IsHardwareDetected &&
                fingerprintManager.HasEnrolledFingerprints &&
                keyguardManager.IsKeyguardSecure && // User has enabled lockscreen -> requirement for fingerprint
                ApplicationSettings.IsFingerprintEnabled)
            {
                AuthenticateWithFingerprint();
            }
            else
            {
                _fingerprintIcon.Visibility = ViewStates.Gone;
            }


            _editPin.AfterTextChanged += (sender, args) =>
            {
                if (_editPin.Text?.Length == ApplicationSettings.PinLength)
                {
                    if (ApplicationSettings.IsPinValid(_editPin.Text))
                    {
                        Locked = false;
                        Finish();
                    }
                    else
                    {
                        _editPin.StartAnimation(AnimationUtils.LoadAnimation(this, Resource.Animation.shake));
                        _editPin.Text = string.Empty;
                        _editPin.Error = Resources.GetString(Resource.String.PinWrong);
                        Task.Run(() => Task.Delay(3000).ContinueWith(t => RunOnUiThread(() => _editPin.Error = null)));
                    }
                }
            };

        }

        private void AuthenticateWithFingerprint()
        {
            _cancellationSignal = new CancellationSignal();

            const string keyName = "MyccFingerprintKey";

            var keyGenerator = KeyGenerator.GetInstance(KeyProperties.KeyAlgorithmAes);
            // ReSharper disable once BitwiseOperatorOnEnumWithoutFlags
            keyGenerator.Init(new KeyGenParameterSpec.Builder(keyName, KeyStorePurpose.Decrypt | KeyStorePurpose.Encrypt)
            .SetBlockModes(KeyProperties.BlockModeCbc).SetUserAuthenticationRequired(true).SetEncryptionPaddings(KeyProperties.EncryptionPaddingPkcs7)
            .Build());

            var key = keyGenerator.GenerateKey();

            var cipher = Cipher.GetInstance(KeyProperties.KeyAlgorithmAes + "/" + KeyProperties.BlockModeCbc + "/" + KeyProperties.EncryptionPaddingPkcs7);
            cipher.Init(CipherMode.EncryptMode, key);

            var dialog = new FingerprintDialog
            {
                OnCancel = () => _cancellationSignal.Cancel()
            };

            var callback = new CallbackManager
            {
                OnSuccess = () =>
                {
                    dialog.Dismiss();
                    _cancellationSignal.Cancel();
                    Locked = false;
                    Finish();
                },
                OnError = () => dialog.AlertError(this)
            };

            dialog.Show(SupportFragmentManager, "FingerprintDialog");

            var fingerprintManager = FingerprintManagerCompat.From(this);
            fingerprintManager.Authenticate(new FingerprintManagerCompat.CryptoObject(cipher), 0, _cancellationSignal, callback, null);
        }

        private class CallbackManager : FingerprintManagerCompat.AuthenticationCallback
        {
            public Action OnSuccess;
            public Action OnError;


            public override void OnAuthenticationSucceeded(FingerprintManagerCompat.AuthenticationResult result)
            {
                base.OnAuthenticationSucceeded(result);
                OnSuccess?.Invoke();
            }

            public override void OnAuthenticationError(int errMsgId, ICharSequence errString)
            {
                base.OnAuthenticationError(errMsgId, errString);
                OnError?.Invoke();
            }

            public override void OnAuthenticationFailed()
            {
                base.OnAuthenticationFailed();
                OnError?.Invoke();
            }
        }

        public override void OnBackPressed()
        {
            // Ignore
        }
    }
}