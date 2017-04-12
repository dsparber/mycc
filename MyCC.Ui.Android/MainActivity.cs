using Android.App;
using Android.Content.Res;
using Android.OS;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using MyCC.Ui.Android.Views.Fragments;
using ActionBarDrawerToggle = Android.Support.V7.App.ActionBarDrawerToggle;

namespace MyCC.Ui.Android
{
    [Activity(Label = "MyCC.Ui.Android", MainLauncher = true, Icon = "@drawable/icon", Theme = "@style/MyCC")]
    public class MainActivity : AppCompatActivity
    {
        private string[] _items;
        private DrawerLayout _drawerLayout;
        private ListView _drawerList;
        private ActionBarDrawerToggle _drawerToggle;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Main);
            SupportActionBar.Elevation = 2;

            _items = new[] { "Rates", "Table", "Graph", "Settings" };
            _drawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            _drawerList = FindViewById<ListView>(Resource.Id.left_drawer);
            var drawerPanel = FindViewById<LinearLayout>(Resource.Id.drawer_panel);

            _drawerList.Adapter = new ArrayAdapter<string>(this, Resource.Layout.item_navigation_drawer, _items);
            _drawerList.ItemClick += (sender, args) =>
            {
                global::Android.Support.V4.App.Fragment fragment;
                switch (args.Position)
                {
                    case 0: fragment = new RatesContainerFragment(); break;
                    case 1: fragment = new AssetsTableContainerFragment(); break;
                    case 2: fragment = new AssetsGraphContainerFragment(); break;
                    default: fragment = new SettingsContainerFragment(); break;
                }

                SupportFragmentManager.BeginTransaction().Replace(Resource.Id.content_frame, fragment).Commit();

                _drawerList.SetItemChecked(args.Position, true);
                SupportActionBar.Title = _items[args.Position];
                _drawerLayout.CloseDrawer(drawerPanel);
            };

            _drawerToggle = new ActionBarDrawerToggle(this, _drawerLayout,
                Resource.String.abc_action_bar_home_description, Resource.String.abc_action_bar_home_description);

            _drawerLayout.AddDrawerListener(_drawerToggle);

            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetHomeButtonEnabled(true);
        }

        protected override void OnPostCreate(Bundle savedInstanceState)
        {
            base.OnPostCreate(savedInstanceState);
            _drawerToggle.SyncState();
        }

        public override void OnConfigurationChanged(Configuration newConfig)
        {
            base.OnConfigurationChanged(newConfig);
            _drawerToggle.OnConfigurationChanged(newConfig);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            return _drawerToggle.OnOptionsItemSelected(item) || base.OnOptionsItemSelected(item);
        }
    }
}

