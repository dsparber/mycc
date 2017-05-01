using System;
using System.Threading.Tasks;
using Android.Animation;
using Android.OS;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using MyCC.Ui.Messages;
using Fragment = Android.Support.V4.App.Fragment;

namespace MyCC.Ui.Android.Views.Fragments
{
    public class ProgressFragment : Fragment
    {
        private ProgressBar _progressBar;
        private ObjectAnimator _animator;


        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.fragment_progress, container, false);

            _progressBar = view.FindViewById<ProgressBar>(Resource.Id.progressbar);

            Messaging.Status.Progress.Subscribe(this, SetProgress);

            return view;
        }

        private void SetProgress(double percentage)
        {
            if (_progressBar == null || Activity == null) return;

            Activity.RunOnUiThread(() =>
            {
                var progress = (int)Math.Round(percentage * 100, 0);

                _progressBar.Visibility = progress > 0 ? ViewStates.Visible : ViewStates.Invisible;

                _animator = ObjectAnimator.OfInt(_progressBar, "progress", progress);
                _animator.SetDuration(500);
                _animator.SetInterpolator(new DecelerateInterpolator());
                _animator.Start();

                if (progress == 100) Task.Delay(500).ContinueWith(t => SetProgress(0));
            });
        }
    }
}