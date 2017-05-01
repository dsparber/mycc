using Android.App;
using Android.Content;
using Android.Net;

namespace MyCC.Ui.Android.Helpers
{
    public class ConnectivityStatus
    {
        private readonly ConnectivityManager _connectivityManager;

        private ConnectivityStatus()
        {
            _connectivityManager = (ConnectivityManager)Application.Context.GetSystemService(Context.ConnectivityService);
            Application.Context.RegisterReceiver(new ConnectivityBroadcastReceiver(), new IntentFilter("android.net.conn.CONNECTIVITY_CHANGE"));
        }

        private static readonly ConnectivityStatus Instance = new ConnectivityStatus();

        public static bool IsConnected => Instance._connectivityManager?.ActiveNetworkInfo?.IsConnected ?? false;
    }


}