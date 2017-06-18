using System;
using System.Threading.Tasks;
using Android.Animation;
using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using Android.Content;
using MyCC.Ui.Android.Helpers;
using MyCC.Ui.Messages;

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
            _progressBar.Indeterminate = true;

            void StartLoading(bool b)
            {
                if (!b || _startedLoading) return;

                RunOnUiThread(() =>
                {
                    _offlineView.Visibility = ViewStates.Gone;
                    _progressView.Visibility = ViewStates.Visible;
                });

                _startedLoading = true;
                Task.Run(LoadInitalData).ConfigureAwait(false);
            }

            if (!ConnectivityStatus.IsConnected)
            {
                _offlineView.Visibility = ViewStates.Visible;
                _progressView.Visibility = ViewStates.Gone;
            }

            StartLoading(ConnectivityStatus.IsConnected);
            Messaging.Status.Network.Subscribe(this, StartLoading);
        }

        private async Task LoadInitalData()
        {
            await UiUtils.Prepare.Prepare(tuple => SetStatus(tuple.progress, tuple.infoText));

            RunOnUiThread(() =>
            {
                var intent = new Intent(this, typeof(MainActivity));
                intent.PutExtra(MainActivity.ExtraInitialisedBefore, true);
                StartActivity(intent);
            });
        }

        private void SetStatus(double percentage, string text)
        {
            RunOnUiThread(() =>
            {
                _progressTextView.Text = text;

                var progress = (int)Math.Round(percentage * 100, 0);

                _progressBar.Indeterminate = false;
                var animator = ObjectAnimator.OfInt(_progressBar, "progress", progress);
                animator.SetDuration(2500);
                animator.SetInterpolator(new DecelerateInterpolator());
                animator.Start();
            });
        }
    }
}