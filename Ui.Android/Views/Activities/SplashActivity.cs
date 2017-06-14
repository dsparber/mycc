using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using HockeyApp.Android;
using HockeyApp.Android.Metrics;
using MyCC.Core.Database;
using MyCC.Ui.Android.Helpers;
using Xamarin.Forms;
using ZXing.Mobile;
using Application = Android.App.Application;

namespace MyCC.Ui.Android.Views.Activities
{
    [Activity(Theme = "@style/SplashTheme.Splash", MainLauncher = true, Icon = "@drawable/ic_launcher", NoHistory = true)]
    public class SplashActivity : AppCompatActivity
    {
        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Forms.Init(this, savedInstanceState);
            DatabaseUtil.SqLiteConnection = new SqLiteConnectionAndroid();
            MobileBarcodeScanner.Initialize(Application);
            CrashManager.Register(this, "7792ee5321a64433ace4955a1693cca5");
            MetricsManager.Register(Application, "7792ee5321a64433ace4955a1693cca5");

            if (UiUtils.Prepare.PreparingNeeded)
            {
                StartActivity(new Intent(this, typeof(PreparingAppActivity)));
            }
            else
            {
                await Startup();
            }
        }

        private async Task Startup()
        {
            await UiUtils.Update.LoadNeededDataFromDatabase();

            RunOnUiThread(() =>
            {
                StartActivity(new Intent(Application.Context, typeof(MainActivity)));
                Finish();
            });

            if (ConnectivityStatus.IsConnected) UiUtils.Update.FetchCurrencies();

        }
    }
}