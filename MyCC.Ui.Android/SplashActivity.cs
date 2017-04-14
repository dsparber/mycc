using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using MyCC.Core.Tasks;
using MyCC.Ui.Android.Data;
using MyCC.Ui.Android.Messages;

namespace MyCC.Ui.Android
{
    [Activity(Theme = "@style/SplashTheme.Splash", MainLauncher = true, Icon = "@drawable/ic_launcher", NoHistory = true)]
    public class SplashActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Forms.Forms.Init(this, savedInstanceState);

            var startupWork = new Task(Startup);
            startupWork.Start();
        }

        private async void Startup()
        {
            ViewData.Init(this);
            if (ViewData.Assets.Items == null)
            {
                await ApplicationTasks.LoadEverything(() => { Messaging.Update.AllItems.Send(); });
            }

            StartActivity(new Intent(Application.Context, typeof(MainActivity)));
            Finish();
        }
    }
}