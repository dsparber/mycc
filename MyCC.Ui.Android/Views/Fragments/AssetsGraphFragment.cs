using Android.Content;
using Android.OS;
using Android.Support.V4.App;
using Android.Support.V4.Widget;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using Java.Interop;
using MyCC.Core.Currency.Model;
using MyCC.Ui.Android.Data.Get;
using MyCC.Ui.Android.Messages;
using Newtonsoft.Json;

namespace MyCC.Ui.Android.Views.Fragments
{
    public class AssetsGraphFragment : Fragment
    {
        private Currency _referenceCurrency;

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

            var data = ViewData.AssetsGraph.IsDataAvailable;
            view.FindViewById(Resource.Id.data_container).Visibility = data ? ViewStates.Visible : ViewStates.Gone;
            view.FindViewById(Resource.Id.no_data_text).Visibility = data ? ViewStates.Gone : ViewStates.Visible;

            var webView = view.FindViewById<WebView>(Resource.Id.web_view);

            webView.Settings.JavaScriptEnabled = true;
            webView.Settings.DomStorageEnabled = true;
            webView.SetWebViewClient(new CustomWebViewClient(_referenceCurrency));
            webView.AddJavascriptInterface(new WebViewInterface(Context), "Native");

            webView.LoadUrl("file:///android_asset/pieChart.html");

            var headerData = ViewData.AssetsGraph.Headers?[_referenceCurrency];
            if (headerData != null)
            {
                var header = (HeaderFragment)ChildFragmentManager.FindFragmentById(Resource.Id.header_fragment);
                header.MainText = headerData.MainText;
                header.InfoText = headerData.InfoText;
            }

            var refreshView = view.FindViewById<SwipeRefreshLayout>(Resource.Id.swiperefresh);
            refreshView.Refresh += (sender, args) => Messaging.Request.Assets.Send();

            Messaging.UiUpdate.AssetsGraph.Subscribe(this, () => Activity.RunOnUiThread(() =>
            {
                webView.EvaluateJavascript(ViewData.AssetsGraph.JsDataString(_referenceCurrency), null);
                refreshView.Refreshing = false;
            }));

            return view;
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

                if (!ViewData.AssetsGraph.IsReady) return;

                var js = ViewData.AssetsGraph.JsDataString(_referenceCurrency);
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
                Toast.MakeText(_context, $"Open Account (id = {accountId})", ToastLength.Short).Show();
            }

        }
    }
}