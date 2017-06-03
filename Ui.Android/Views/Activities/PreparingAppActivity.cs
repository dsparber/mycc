using System;
using System.Linq;
using System.Threading.Tasks;
using Android.Animation;
using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using Android.Content;
using MyCC.Core.Currencies;
using MyCC.Core.Currencies.Sources;
using MyCC.Core.Helpers;
using MyCC.Core.Settings;
using MyCC.Ui.Android.Helpers;
using MyCC.Ui.Messages;
using MyCC.Ui.Tasks;

namespace MyCC.Ui.Android.Views.Activities
{
    [Activity(Theme = "@style/LockscreenTheme", NoHistory = true)]
    public class PreparingAppActivity : AppCompatActivity
    {

        private View _progressView;
        private View _offlineView;
        private ProgressBar _progressBar;
        private TextView _progressTextView;
        private bool _startedLoading;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_preparing);

            _progressBar = FindViewById<ProgressBar>(Resource.Id.progress_bar);
            _progressTextView = FindViewById<TextView>(Resource.Id.text_progress);
            _progressView = FindViewById(Resource.Id.layout_progress);
            _offlineView = FindViewById(Resource.Id.layout_no_network);

            Action<bool> startLoading = b =>
            {
                if (!b || _startedLoading) return;

                RunOnUiThread(() =>
                {
                    _offlineView.Visibility = ViewStates.Gone;
                    _progressView.Visibility = ViewStates.Visible;
                });

                _startedLoading = true;
                Task.Run(LoadInitalData).ConfigureAwait(false);
            };

            if (!ConnectivityStatus.IsConnected)
            {
                _offlineView.Visibility = ViewStates.Visible;
                _progressView.Visibility = ViewStates.Gone;
            }

            startLoading(ConnectivityStatus.IsConnected);
            Messaging.Status.Network.Subscribe(this, startLoading);
        }

        private async Task LoadInitalData()
        {
            try
            {
                // STEP 1: Fetch available currencies
                var totalCount = CurrencyStorage.Instance.Sources.Count() * 2;
                var count = 0;

                Action<ICurrencySource> setProgress = source =>
                {
                    count += 1;
                    SetStatus(0.8 * count / totalCount, string.Format(Resources.GetString(Resource.String.LoadingCurrenciesFrom), source.Name));
                };

                await CurrencyStorage.Instance.LoadOnline(setProgress, setProgress);


                // STEP 2: Fetch needed Rates
                await TaskHelper.FetchMissingRates(false, progress => SetStatus(0.8 + progress * 0.2, Resources.GetString(Resource.String.LoadingRates)));

                Messaging.Update.AllItems.Send();
                ApplicationSettings.AppInitialised = true;

                RunOnUiThread(() =>
                {
                    var intent = new Intent(this, typeof(MainActivity));
                    intent.PutExtra(MainActivity.ExtraInitialisedBefore, true);
                    StartActivity(intent);
                });
            }
            catch (Exception e)
            {
                e.LogError();
            }
        }

        private void SetStatus(double percentage, string text)
        {
            RunOnUiThread(() =>
            {
                _progressTextView.Text = text;

                var progress = (int)Math.Round(percentage * 100, 0);


                var animator = ObjectAnimator.OfInt(_progressBar, "progress", progress);
                animator.SetDuration(2500);
                animator.SetInterpolator(new DecelerateInterpolator());
                animator.Start();


            });
        }
    }
}