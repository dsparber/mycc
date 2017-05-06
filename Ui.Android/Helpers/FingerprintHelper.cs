using Android.App;
using Android.Content;
using Android.Support.V4.Hardware.Fingerprint;

namespace MyCC.Ui.Android.Helpers
{
    public static class FingerprintHelper
    {
        public static bool IsFingerprintAvailable
        {
            get
            {
                var fingerprintManager = FingerprintManagerCompat.From(Application.Context);
                var keyguardManager = (KeyguardManager)Application.Context.GetSystemService(Context.KeyguardService);

                return fingerprintManager.IsHardwareDetected &&
                       fingerprintManager.HasEnrolledFingerprints &&
                       keyguardManager.IsKeyguardSecure; // User has enabled lockscreen -> requirement for fingerprint
            }
        }
    }
}