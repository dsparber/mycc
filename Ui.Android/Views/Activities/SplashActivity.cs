using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using HockeyApp.Android;
using HockeyApp.Android.Metrics;
using MyCC.Core.Settings;
using MyCC.Core.Tasks;
using MyCC.Ui.Android.Helpers;
using MyCC.Ui.Messages;
using Xamarin.Forms;
using ZXing.Mobile;
using Application = Android.App.Application;

namespace MyCC.Ui.Android.Views.Activities
{
    [Activity(Theme = "@style/SplashTheme.Splash", MainLauncher = true, Icon = "@drawable/ic_launcher", NoHistory = true)]
    public class SplashActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Forms.Init(this, savedInstanceState);
            MobileBarcodeScanner.Initialize(Application);
            CrashManager.Register(this, "7792ee5321a64433ace4955a1693cca5");
            MetricsManager.Register(Application, "7792ee5321a64433ace4955a1693cca5");
            ApplicationSettings.Migrate();

            if (!ApplicationSettings.AppInitialised)
            {
                StartActivity(new Intent(this, typeof(PreparingAppActivity)));
            }
            else
            {
                var startupWork = new Task(Startup);
                startupWork.Start();
            }
        }

        private async void Startup()
        {
            if (ViewData.ViewData.Assets.Items == null)
            {
                await ApplicationTasks.LoadEverything(() => { Messaging.Update.AllItems.Send(); });
            }

            StartActivity(new Intent(Application.Context, typeof(MainActivity)));

            if (ConnectivityStatus.IsConnected) await ApplicationTasks.FetchCurrenciesAndAvailableRates();

            Finish();
        }
    }
}