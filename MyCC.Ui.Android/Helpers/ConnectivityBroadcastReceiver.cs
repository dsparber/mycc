using Android.Content;
using MyCC.Ui.Android.Messages;

namespace MyCC.Ui.Android.Helpers
{
    public class ConnectivityBroadcastReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            if ("android.net.conn.CONNECTIVITY_CHANGE".Equals(intent.Action))
            {
                Messaging.Status.Network.Send(ConnectivityStatus.IsConnected);
            }
        }
    }
}