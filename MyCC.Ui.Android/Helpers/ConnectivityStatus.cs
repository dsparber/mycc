using Android.Content;
using Android.Net;

namespace MyCC.Ui.Android.Helpers
{
    public class ConnectivityStatus
    {
        private readonly ConnectivityManager _connectivityManager;

        private ConnectivityStatus(Context context)
        {
            _connectivityManager = (ConnectivityManager)context.GetSystemService(Context.ConnectivityService);
        }

        private static ConnectivityStatus _instance;

        public static void Init(Context context)
        {
            _instance = _instance ?? new ConnectivityStatus(context);
        }

        public static bool IsConnected => _instance._connectivityManager.ActiveNetworkInfo.IsConnected;
    }
}