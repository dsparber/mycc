using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.Widget;
using Android.Views;
using Android.Webkit;
using Java.Interop;
using MyCC.Ui.Android.Helpers;
using MyCC.Ui.Android.Views.Activities;
using MyCC.Ui.Messages;
using Fragment = Android.Support.V4.App.Fragment;

namespace MyCC.Ui.Android.Views.Fragments
{
    public class AssetsGraphFragment : Fragment
    {
        private string _referenceCurrencyId;
        private HeaderFragment _header;
        private FooterFragment _footerFragment;


        public AssetsGraphFragment(string referenceCurrencyId)
        {
            _referenceCurrencyId = referenceCurrencyId;
        }

        public AssetsGraphFragment()
        {
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            _referenceCurrencyId = savedInstanceState?.GetString("currency");

            var view = inflater.Inflate(Resource.Layout.fragment_assets_graph, container, false);

            SetVisibleElements(view);

            var webView = view.FindViewById<WebView>(Resource.Id.web_view);

            webView.Settings.JavaScriptEnabled = true;
            webView.Settings.DomStorageEnabled = true;
            webView.SetWebViewClient(new CustomWebViewClient(_referenceCurrencyId));
            webView.AddJavascriptInterface(new WebViewInterface(Context), "Native");

            webView.LoadUrl("file:///android_asset/pieChart.html");

            var headerData = UiUtils.Get.Assets.HeaderFor(_referenceCurrencyId);
            _header = (HeaderFragment)ChildFragmentManager.FindFragmentById(Resource.Id.header_fragment);
            _header.Data = headerData;

            _footerFragment = (FooterFragment)ChildFragmentManager.FindFragmentById(Resource.Id.footer_fragment);
            _footerFragment.LastUpdate = UiUtils.Get.Assets.LastUpdate;

            var refreshView = view.FindViewById<SwipeRefreshLayout>(Resource.Id.swiperefresh);
            refreshView.Refresh += (sender, args) => UiUtils.Update.FetchAllAssetsAndRates();

            Messaging.UiUpdate.AssetsGraph.Subscribe(this, () =>
            {
                if (Activity == null) return;
                Activity.RunOnUiThread(() =>
                {
                    _header.Data = UiUtils.Get.Rates.HeaderFor(_referenceCurrencyId);
                    _footerFragment.LastUpdate = UiUtils.Get.Assets.LastUpdate;
                    var js = UiUtils.Get.Assets.GrapItemsJsFor(_referenceCurrencyId);
                    webView.LoadUrl($"javascript:{js}", null);
                    SetVisibleElements(view);
                    refreshView.Refreshing = false;
                });
            });

            view.FindViewById<FloatingActionButton>(Resource.Id.button_add).Click += (sender, args) =>
            {
                StartActivity(new Intent(Application.Context, typeof(AddSourceActivity)));
            };

            var activityRootView = view.FindViewById(Resource.Id.fragment_root);
            activityRootView.ViewTreeObserver.GlobalLayout += (sender, args) =>
            {
                if (IsDetached || !IsAdded) return;
                ChildFragmentManager.SetFragmentVisibility(_header, activityRootView.Height > 360.DpToPx());
            };

            return view;
        }

        private static void SetVisibleElements(View view)
        {
            var data = UiUtils.Get.Assets.IsGraphDataAvailable;
            view.FindViewById(Resource.Id.data_container).Visibility = data ? ViewStates.Visible : ViewStates.Gone;
            view.FindViewById(Resource.Id.no_data_text).Visibility = data ? ViewStates.Gone : ViewStates.Visible;
        }

        public override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);
            outState.PutString("currency", _referenceCurrencyId);
        }

        private class CustomWebViewClient : WebViewClient
        {
            private readonly string _referenceCurrencyId;

            public CustomWebViewClient(string referenceCurrencyId)
            {
                _referenceCurrencyId = referenceCurrencyId;
            }

            public override void OnPageFinished(WebView view, string url)
            {
                base.OnPageFinished(view, url);

                if (!UiUtils.Get.Assets.IsGraphDataAvailable) return;

                var js = UiUtils.Get.Assets.GrapItemsJsFor(_referenceCurrencyId);
                view.LoadUrl($"javascript:{js}", null);
            }
        }

        private class WebViewInterface : Java.Lang.Object
        {
            private readonly Context _context;


            public WebViewInterface(Context c)
            {
                _context = c;
            }

            // ReSharper disable once UnusedMember.Local
            [Export("OpenView")]
            [JavascriptInterface]
            public void OpenView(int accountId)
            {
                var intent = new Intent(_context, typeof(AccountDetailActivity));
                intent.PutExtra(AccountDetailActivity.ExtraAccountId, accountId);
                _context.StartActivity(intent);
            }

        }
    }
}