using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.Content.Res;
using Android.OS;
using Android.Support.V4.Widget;
using Android.Views;
using Android.Widget;
using MyCC.Core.Settings;
using MyCC.Core.Types;
using MyCC.Ui.Android.Helpers;
using MyCC.Ui.Android.Views.Fragments;
using ActionBarDrawerToggle = Android.Support.V7.App.ActionBarDrawerToggle;
using Fragment = Android.Support.V4.App.Fragment;
using MyCC.Ui.Messages;
using MyCC.Ui.Tasks;

namespace MyCC.Ui.Android.Views.Activities
{
    [Activity(Label = "@string/AppName", Icon = "@drawable/ic_launcher", Theme = "@style/MyCC")]
    public class MainActivity : MyccActivity
    {
        private string[] _items;
        private DrawerLayout _drawerLayout;
        private ListView _drawerList;
        private ActionBarDrawerToggle _drawerToggle;
        private int? _position;

        private ViewPagerFragment _assetsTableFragment;
        private ViewPagerFragment _assetsGraphFragment;

        private List<RatesFragment> _ratesFragments;

        private IMenuItem _editItem;
        private IMenuItem _doneItem;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            _position = bundle?.GetInt("position");

            SetContentView(Resource.Layout.Main);

            CreateDrawerLayout();

            Task.Run(() =>
            {
                TaskHelper.FetchMissingRates();
                if (ApplicationSettings.AutoRefreshOnStartup && ConnectivityStatus.IsConnected)
                {
                    TaskHelper.UpdateAllAssetsAndRates();
                }
            });
        }

        private void CreateDrawerLayout()
        {
            _items = new[]
            {
                Resources.GetString(Resource.String.Rates),
                Resources.GetString(Resource.String.Table),
                Resources.GetString(Resource.String.Graph),
                Resources.GetString(Resource.String.Settings)
            };
            _drawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            _drawerList = FindViewById<ListView>(Resource.Id.left_drawer);
            var drawerPanel = FindViewById<LinearLayout>(Resource.Id.drawer_panel);

            _drawerList.Adapter = new ArrayAdapter<string>(this, Resource.Layout.item_navigation_drawer, _items);
            _drawerList.ItemClick += (sender, args) =>
            {
                _position = args.Position;
                SetFragment();

                _drawerLayout.CloseDrawer(drawerPanel);
                _drawerList.SetItemChecked(args.Position, true);
                _editItem?.SetVisible(_position == 0);
                _doneItem?.SetVisible(false);
            };

            _drawerToggle = new ActionBarDrawerToggle(this, _drawerLayout,
                Resource.String.abc_action_bar_home_description, Resource.String.abc_action_bar_home_description);

            _drawerLayout.AddDrawerListener(_drawerToggle);

            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetHomeButtonEnabled(true);

            var startPage = ApplicationSettings.DefaultStartupPage;

            _position = _position ?? (startPage == StartupPage.RatesView ? 0 : startPage == StartupPage.TableView ? 1 : 2);
            _editItem?.SetVisible(_position == 0);
            SetFragment();

            Messaging.SettingChange.MainCurrencies.Subscribe(this, SetFragment);
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            _editItem = menu.Add(0, 0, 0, Resources.GetString(Resource.String.Edit));
            _editItem.SetIcon(Resource.Drawable.ic_edit).SetShowAsAction(ShowAsAction.Always);
            _editItem?.SetVisible(_position == 0);

            _doneItem = menu.Add(0, 0, 0, Resources.GetString(Resource.String.Done));
            _doneItem.SetIcon(Resource.Drawable.ic_action_done).SetShowAsAction(ShowAsAction.Always);
            _doneItem?.SetVisible(false);

            return true;
        }

        private void SetFragment()
        {
            Fragment fragment;
            switch (_position)
            {
                case 0:
                    _ratesFragments = ApplicationSettings.MainCurrencies.Select(c => new RatesFragment(c)).ToList();
                    foreach (var f in _ratesFragments) f.EditingEnabled = false;
                    fragment = new ViewPagerFragment(_ratesFragments.OfType<Fragment>().ToList(), ApplicationSettings.MainCurrencies.IndexOf(ApplicationSettings.StartupCurrencyRates));
                    ((ViewPagerFragment)fragment).PositionChanged = pos =>
                    {
                        if (pos >= ApplicationSettings.MainCurrencies.Count) return;
                        ApplicationSettings.StartupCurrencyRates = ApplicationSettings.MainCurrencies[pos];
                    };
                    break;
                case 1:
                    var assetsfragments = ApplicationSettings.MainCurrencies.Select(c => new AssetsTableFragment(c) as Fragment).ToList();
                    _assetsTableFragment = new ViewPagerFragment(assetsfragments, ApplicationSettings.MainCurrencies.IndexOf(ApplicationSettings.StartupCurrencyAssets));
                    _assetsTableFragment.PositionChanged = pos =>
                    {
                        if (pos >= ApplicationSettings.MainCurrencies.Count) return;

                        ApplicationSettings.StartupCurrencyAssets = ApplicationSettings.MainCurrencies[pos];
                        if (_assetsGraphFragment != null) _assetsGraphFragment.Position = pos;
                    };
                    fragment = _assetsTableFragment;
                    break;
                case 2:
                    var graphfragments = ApplicationSettings.MainCurrencies.Select(c => new AssetsGraphFragment(c) as Fragment).ToList();
                    _assetsGraphFragment = new ViewPagerFragment(graphfragments, ApplicationSettings.MainCurrencies.IndexOf(ApplicationSettings.StartupCurrencyAssets));
                    _assetsGraphFragment.PositionChanged = pos =>
                    {
                        if (pos >= ApplicationSettings.MainCurrencies.Count) return;

                        ApplicationSettings.StartupCurrencyAssets = ApplicationSettings.MainCurrencies[pos];
                        if (_assetsTableFragment != null) _assetsTableFragment.Position = pos;
                    };
                    fragment = _assetsGraphFragment;
                    break;
                default: fragment = new SettingsContainerFragment(); break;
            }

            SupportFragmentManager.BeginTransaction().Replace(Resource.Id.content_frame, fragment).Commit();

            SupportActionBar.Title = _items[_position ?? 0];
        }

        protected override void OnPostCreate(Bundle savedInstanceState)
        {
            base.OnPostCreate(savedInstanceState);
            _drawerToggle.SyncState();
        }

        protected override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);
            outState.PutInt("position", _position ?? 0);
        }

        public override void OnConfigurationChanged(Configuration newConfig)
        {
            base.OnConfigurationChanged(newConfig);
            _drawerToggle.OnConfigurationChanged(newConfig);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (string.Equals(item?.TitleFormatted?.ToString(), Resources.GetString(Resource.String.Edit)))
            {
                foreach (var f in _ratesFragments ?? new List<RatesFragment>()) f.EditingEnabled = true;
                _editItem?.SetVisible(false);
                _doneItem?.SetVisible(true);
                return true;
            }
            if (string.Equals(item?.TitleFormatted?.ToString(), Resources.GetString(Resource.String.Done)))
            {
                foreach (var f in _ratesFragments ?? new List<RatesFragment>()) f.EditingEnabled = false;
                _editItem?.SetVisible(true);
                _doneItem?.SetVisible(false);
                return true;
            }
            return _drawerToggle.OnOptionsItemSelected(item) || base.OnOptionsItemSelected(item);
        }
    }
}

