using System;
using Android.Content;
using Android.Hardware;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using MyCC.Core.Settings;
using MyCC.Ui.Android.Helpers;

namespace MyCC.Ui.Android.Views.Activities
{
    public abstract class MyccActivity : AppCompatActivity
    {
        private static int _runningActivities;
        private static ShakeRecognizer _shakeRecognizer;
        private static DateTime _lastStop;
        protected static bool Locked = ApplicationSettings.IsPinSet;
        public static MyccActivity CurrentInstance;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
            {
                Window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);
            }

            if (SupportActionBar == null) return;

            SupportActionBar.Elevation = 3;
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (string.IsNullOrWhiteSpace(item?.TitleFormatted?.ToString()))
            {
                Finish();
            }
            return true;
        }

        protected override void OnStart()
        {
            base.OnStart();

            CurrentInstance = this;

            var millis = DateTime.Now.Subtract(_lastStop).TotalMilliseconds;
            if (!(this is LockscreenActivity))
            {
                if (Locked || _runningActivities == 0 && millis > 2500 && ApplicationSettings.IsPinSet)
                {
                    Locked = true;
                    var intent = new Intent(this, typeof(LockscreenActivity));
                    intent.SetFlags(ActivityFlags.NoAnimation);
                    StartActivity(intent);
                }
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
                    if (!ApplicationSettings.IsPinSet || !ApplicationSettings.LockByShaking) return;

                    Locked = true;
                    var intent = new Intent(this, typeof(LockscreenActivity));
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
                _lastStop = DateTime.Now;
                var sensorManager = (SensorManager)GetSystemService(SensorService);
                sensorManager.UnregisterListener(_shakeRecognizer);
                _shakeRecognizer = null;
            }

        }
    }
}