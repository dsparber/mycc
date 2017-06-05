﻿using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using HockeyApp.Android;
using HockeyApp.Android.Metrics;
using MyCC.Core.Currencies;
using MyCC.Core.Database;
using MyCC.Core.Preperation;
using MyCC.Core.Settings;
using MyCC.Core.Tasks;
using MyCC.Ui.Android.Helpers;
using MyCC.Ui.Get;
using MyCC.Ui.Messages;
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
            ViewData.Init();
            MobileBarcodeScanner.Initialize(Application);

            if (Prepare.PreparingNeeded)
            {
                Prepare.ExecutePreperations();
                if (Prepare.AsyncExecutePreperations != null) await Prepare.AsyncExecutePreperations;
            }
            if (Migrate.MigrationsNeeded) await Migrate.ExecuteMigratations();

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
            await CurrencyStorage.Instance.LoadFromDatabase();
            await ApplicationTasks.LoadEverything();
            Messaging.Update.AllItems.Send();

            RunOnUiThread(() =>
            {
                StartActivity(new Intent(Application.Context, typeof(MainActivity)));
                Finish();
            });

            if (ConnectivityStatus.IsConnected) await ApplicationTasks.FetchCurrencies();

        }
    }
}