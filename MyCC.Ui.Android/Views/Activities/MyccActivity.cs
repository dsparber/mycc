using Android.Content;
using Android.Hardware;
using Android.OS;
using Android.Support.V7.App;
using MyCC.Core.Settings;
using MyCC.Ui.Android.Helpers;

namespace MyCC.Ui.Android.Views.Activities
{
    public abstract class MyccActivity : AppCompatActivity
    {
        private static int _runningActivities;
        private static ShakeRecognizer _shakeRecognizer;

        protected override void OnStart()
        {
            base.OnStart();
            if (_runningActivities == 0 && ApplicationSettings.IsPinSet)
            {
                var intent = new Intent(this, typeof(PasswordActivity));
                intent.SetFlags(ActivityFlags.NoAnimation);
                StartActivity(intent);
            }
            _runningActivities += 1;


            if (_shakeRecognizer != null) return;

            var sensorManager = (SensorManager)GetSystemService(SensorService);
            var accelerometer = sensorManager.GetDefaultSensor(SensorType.Accelerometer);
            if (accelerometer == null) return;

            _shakeRecognizer = new ShakeRecognizer
            {
                OnShake = () =>
                {
                    var intent = new Intent(this, typeof(PasswordActivity));
                    intent.SetFlags(ActivityFlags.NoAnimation);
                    StartActivity(intent);
                }
            };
            sensorManager.RegisterListener(_shakeRecognizer, accelerometer, SensorDelay.Ui);
        }

        protected override void OnStop()
        {
            base.OnStop();
            _runningActivities -= 1;

            if (_runningActivities == 0 && _shakeRecognizer != null)
            {
                var sensorManager = (SensorManager)GetSystemService(SensorService);
                sensorManager.UnregisterListener(_shakeRecognizer);
                _shakeRecognizer = null;
            }

        }
    }
}