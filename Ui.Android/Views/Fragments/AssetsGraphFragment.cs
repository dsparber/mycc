using System;
using System.Linq;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.Widget;
using Android.Views;
using Android.Webkit;
using Java.Interop;
using MyCC.Core.Currencies.Model;
using MyCC.Core.Settings;
using MyCC.Ui.Android.Helpers;
using MyCC.Ui.Android.Views.Activities;
using MyCC.Ui.Messages;
using Newtonsoft.Json;
using Fragment = Android.Support.V4.App.Fragment;

namespace MyCC.Ui.Android.Views.Fragments
{
    public class AssetsGraphFragment : Fragment
    {
        private Currency _referenceCurrency;
        private HeaderFragment _header;
        private FooterFragment _footerFragment;


        public AssetsGraphFragment(Currency referenceCurrency)
        {
            _referenceCurrency = referenceCurrency;
        }

        public AssetsGraphFragment()
        {
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var saved = savedInstanceState?.GetString("currency");
            if (saved != null)
            {
                _referenceCurrency = JsonConvert.DeserializeObject<Currency>(saved);
            }

            var view = inflater.Inflate(Resource.Layout.fragment_assets_graph, container, false);

            SetVisibleElements(view);

            var webView = view.FindViewById<WebView>(Resource.Id.web_view);

            webView.Settings.JavaScriptEnabled = true;
            webView.Settings.DomStorageEnabled = true;
            webView.SetWebViewClient(new CustomWebViewClient(_referenceCurrency));
            webView.AddJavascriptInterface(new WebViewInterface(Context), "Native");

            webView.LoadUrl("file:///android_asset/pieChart.html");

            var headerData = ViewData.ViewData.Assets.Headers?[_referenceCurrency];
            _header = (HeaderFragment)ChildFragmentManager.FindFragmentById(Resource.Id.header_fragment);
            _header.Data = headerData;

            _footerFragment = (FooterFragment)ChildFragmentManager.FindFragmentById(Resource.Id.footer_fragment);
            _footerFragment.LastUpdate = ViewData.ViewData.Assets.LastUpdate?[_referenceCurrency] ?? DateTime.MinValue;

            var refreshView = view.FindViewById<SwipeRefreshLayout>(Resource.Id.swiperefresh);
            refreshView.Refresh += (sender, args) => Messaging.Request.AllAssetsAndRates.Send();

            Messaging.UiUpdate.AssetsGraph.Subscribe(this, () =>
            {
                if (Activity == null) return;
                Activity.RunOnUiThread(() =>
                {
                    if (!ViewData.ViewData.Assets.IsGraphDataAvailable) return;
                    if (!ApplicationSettings.MainCurrencies.Contains(_referenceCurrency.Id)) return;
                    if (!ViewData.ViewData.Assets.Headers.TryGetValue(_referenceCurrency, out headerData)) return;

                    _header.Data = headerData;
                    _footerFragment.LastUpdate = ViewData.ViewData.Assets.LastUpdate[_referenceCurrency];
                    var js = ViewData.ViewData.Assets.JsDataString(_referenceCurrency);
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
                ChildFragmentManager.SetFragmentVisibility(_header, activityRootView.Height > 400.DpToPx());
            };

            return view;
        }

        private static void SetVisibleElements(View view)
        {
            var data = ViewData.ViewData.Assets.IsGraphDataAvailable;
            view.FindViewById(Resource.Id.data_container).Visibility = data ? ViewStates.Visible : ViewStates.Gone;
            view.FindViewById(Resource.Id.no_data_text).Visibility = data ? ViewStates.Gone : ViewStates.Visible;
        }

        public override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);
            outState.PutString("currency", JsonConvert.SerializeObject(_referenceCurrency));
        }

        private class CustomWebViewClient : WebViewClient
        {
            private readonly Currency _referenceCurrency;

            public CustomWebViewClient(Currency referenceCurrency)
            {
                _referenceCurrency = referenceCurrency;
            }

            public override void OnPageFinished(WebView view, string url)
            {
                base.OnPageFinished(view, url);

                if (!ViewData.ViewData.Assets.IsGraphDataAvailable) return;

                var js = ViewData.ViewData.Assets.JsDataString(_referenceCurrency);
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