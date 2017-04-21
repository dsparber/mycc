using System;
using Android.Hardware;

namespace MyCC.Ui.Android.Helpers
{
    // http://jasonmcreynolds.com/?p=388

    public class ShakeRecognizer : Java.Lang.Object, ISensorEventListener
    {
        private const float ShakeThresholdGravity = 2.7F;
        private const int ShakeSlopTimeMs = 500;

        public Action OnShake;
        private long _shakeTimestamp;

        public void OnAccuracyChanged(Sensor sensor, SensorStatus accuracy)
        {
            // Ignored
        }

        public void OnSensorChanged(SensorEvent e)
        {
            var x = e.Values[0];
            var y = e.Values[1];
            var z = e.Values[2];


            var gX = x / SensorManager.GravityEarth;
            var gY = y / SensorManager.GravityEarth;
            var gZ = z / SensorManager.GravityEarth;

            var gForce = Math.Sqrt(gX * gX + gY * gY + gZ * gZ);

            if (gForce > ShakeThresholdGravity)
            {
                var now = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

                // ignore shake events too close to each other (500ms)
                if (_shakeTimestamp + ShakeSlopTimeMs > now) return;

                _shakeTimestamp = now;
                OnShake.Invoke();
            }
        }
    }
}