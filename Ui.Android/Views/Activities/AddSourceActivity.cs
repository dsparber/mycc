using System.Threading;
using Android.App;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Android.Views;
using MyCC.Core.Currencies.Models;
using MyCC.Ui.Android.Helpers;
using MyCC.Ui.Android.Views.Adapter;
using MyCC.Ui.Android.Views.Fragments;
using MyCC.Ui.Android.Views.Fragments.AddSource;
using MyCC.Ui.Edit;
using MyCC.Ui.Get;

namespace MyCC.Ui.Android.Views.Activities
{
    [Activity(Label = "@string/AddSource", Theme = "@style/MyCC")]
    public class AddSourceActivity : MyccActivity
    {
        private AddSourcePagerAdapter _pagerAdapter;
        private ViewPager _viewPager;
        private HeaderFragment _header;

        private string _name;

        private int _savingClickedCount;
        private bool _willFinish;

        public string Name
        {
            get => _name;
            set
            {
                _name = (value ?? string.Empty).Replace("\n", string.Empty);
                if (_header != null) _header.MainText = string.IsNullOrWhiteSpace(_name) ? Resources.GetString(Resource.String.Unnamed) : _name;
            }
        }

        public Currency Currency;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_add_source);


            _header = (HeaderFragment)SupportFragmentManager.FindFragmentById(Resource.Id.header_fragment);

            _viewPager = FindViewById<ViewPager>(Resource.Id.viewpager);
            _pagerAdapter = new AddSourcePagerAdapter(SupportFragmentManager, this);
            _viewPager.Adapter = _pagerAdapter;
            _viewPager.PageSelected += (sender, args) => _header.InfoText = _pagerAdapter.GetPageTitle(args.Position);

            Name = null;
            _header.InfoText = _pagerAdapter.GetPageTitle(_viewPager.CurrentItem);

            var tabLayout = FindViewById<TabLayout>(Resource.Id.sliding_tabs);
            tabLayout.SetupWithViewPager(_viewPager);

            var activityRootView = FindViewById(Resource.Id.view_root);
            activityRootView.ViewTreeObserver.GlobalLayout += (sender, args) => SupportFragmentManager.SetFragmentVisibility(_header, activityRootView.Height > 480.DpToPx());
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            var item = menu.Add(0, 0, 0, Resources.GetString(Resource.String.Save));
            item.SetIcon(Resource.Drawable.ic_action_done).SetShowAsAction(ShowAsAction.Always);

            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {

            if (item.TitleFormatted == null)
            {
                Finish();
            }
            else
            {
                Interlocked.Increment(ref _savingClickedCount);
                if (_savingClickedCount > 1 || _willFinish) return true;
                var savingDialog = this.GetLoadingDialog(null, Resource.String.SavingSource);
                Save(savingDialog);
            }
            return true;
        }


        private async void Save(ProgressDialog progressDialog)
        {

            var fragment = _pagerAdapter.GetFragment(_viewPager.CurrentItem);

            if (!fragment.EntryComplete)
            {
                progressDialog.Dismiss();
                this.ShowInfoDialog(Resource.String.Error, Resource.String.VerifyInput);
                _savingClickedCount = 0;
                return;
            }

            if (fragment is AddSourceFragment.Repository)
            {

                if (_savingClickedCount > 1) return;
                var result = await UiUtils.Edit.Add(((AddSourceFragment.Repository)fragment).GetRepository(),
                    alreadyAdded:
                    () => this.ShowInfoDialog(Resource.String.Error, Resource.String.RepositoryAlreadyAdded),
                    testingStarted: () => progressDialog.SetMessage(Resources.GetString(Resource.String.Testing)),
                    testingFailed: () => this.ShowInfoDialog(Resource.String.Error, Resource.String.FetchingNoSuccessText));
                _willFinish = result;
                if (result) Finish();
            }
            else
            {
                var account = ((AddSourceFragment.Account)fragment).GetAccount();
                if (_savingClickedCount > 1) return;
                await UiUtils.Edit.Add(account);
                _willFinish = true;
                Finish();
            }
            _savingClickedCount = 0;
            progressDialog.Dismiss();
        }
    }
}