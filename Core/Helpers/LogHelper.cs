﻿using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace MyCC.Core.Helpers
{
    public static class LogHelper
    {
        public static void LogError(this Exception e)
        {
            Debug.WriteLine(e);
            HockeyApp.MetricsManager.TrackEvent($"{e.GetType().Name}: {e.Message}",
                        new Dictionary<string, string> { { "error", e.ToString() } },
                        new Dictionary<string, double> { { "time", DateTime.Now.Ticks } });
        }
    }
}