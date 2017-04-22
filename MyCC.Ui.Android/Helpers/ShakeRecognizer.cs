using System;
using Android.Hardware;

namespace MyCC.Ui.Android.Helpers
{
    // http://jasonmcreynolds.com/?p=388

    public class ShakeRecognizer : Java.Lang.Object, ISensorEventListener
    {
        private const float ShakeThresholdDeltaMin = 1.3F;
        private const float ShakeThresholdGravity = 2.8F;
        private const float ShakeThresholDelta = 3.2F;
        private const float ShakeThresholMinSum = 4.3F;
        private const int ShakeSlopTimeMs = 1000;

        public Action OnShake;
        private long _shakeTimestamp;
        private float _lastX = 1, _lastY = 1, _lastZ = 1;

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

            var now = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

            var dX = Math.Abs(gX - _lastX);
            var dY = Math.Abs(gY - _lastY);
            var dZ = Math.Abs(gZ - _lastZ);
            var dG = Math.Sqrt(dX * dX + dY * dY + dZ * dZ);

            _lastX = gX;
            _lastY = gY;
            _lastZ = gZ;


            if (dG > ShakeThresholdDeltaMin && dG + gForce > ShakeThresholMinSum && (gForce > ShakeThresholdGravity || dG > ShakeThresholDelta))
            {

                // ignore shake events too close to each other (500ms)
                if (_shakeTimestamp + ShakeSlopTimeMs > now) return;

                //if (now - _shakeTimestamp > ResetTime) _count = 0;

                //_count += 1;
                _shakeTimestamp = now;

                //if (_count < MinShakes) return;

                //_count = 0;
                OnShake.Invoke();
            }
        }
    }
}