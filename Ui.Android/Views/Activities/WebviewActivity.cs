using System;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using MyCC.Ui.Android.Helpers;
using MyCC.Ui.Android.Views.Fragments;

namespace MyCC.Ui.Android.Views.Activities
{
    [Activity(Theme = "@style/MyCC")]

    public class WebviewActivity : MyccActivity
    {
        public const string KeyShowVersionHeader = "showVersionHeader";
        public const string KeyOpenLinksInNewActivity = "openLinksInNewActivity";
        public const string KeyUrl = "url";
        public const string KeyTitle = "title";

        private string _url;
        private string _title;
        private bool _showVersionHeader;
        private bool _openLinksInNewActivity;

        private WebView _webView;
        private HeaderFragment _header;
        private ProgressBar _progressBar;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            savedInstanceState = savedInstanceState ?? new Bundle();
            SetContentView(Resource.Layout.activity_webview);

            SupportActionBar.Elevation = 3;
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);

            _openLinksInNewActivity = savedInstanceState.GetBoolean(KeyOpenLinksInNewActivity, Intent?.GetBooleanExtra(KeyOpenLinksInNewActivity, false) ?? false);
            _showVersionHeader = savedInstanceState.GetBoolean(KeyShowVersionHeader, Intent?.GetBooleanExtra(KeyShowVersionHeader, false) ?? false);
            _title = savedInstanceState.GetString(KeyTitle, Intent?.GetStringExtra(KeyTitle));
            _url = savedInstanceState.GetString(KeyUrl, Intent?.GetStringExtra(KeyUrl));

            if (string.IsNullOrWhiteSpace(_url)) throw new NullReferenceException("A url must be specified and passed as an extra!");

            SupportActionBar.Title = _title ?? new Uri(_url).Host;

            _header = (HeaderFragment)SupportFragmentManager.FindFragmentById(Resource.Id.header_fragment);
            _webView = FindViewById<WebView>(Resource.Id.webview);
            _progressBar = FindViewById<ProgressBar>(Resource.Id.progressbar);

            _header.InfoText = PackageManager.GetPackageInfo(PackageName, 0).VersionName;
            SupportFragmentManager.SetFragmentVisibility(_header, _showVersionHeader);

            _webView.Settings.JavaScriptEnabled = true;
            _webView.Settings.DomStorageEnabled = true;
            _webView.SetWebViewClient(new CustomWebViewClient(this, _progressBar, _openLinksInNewActivity));
            _webView.LoadUrl(_url);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            Finish();
            return true;
        }

        protected override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);
            outState.PutString(KeyUrl, _url);
            outState.PutString(KeyUrl, _title);
            outState.PutBoolean(KeyUrl, _showVersionHeader);
            outState.PutBoolean(KeyUrl, _openLinksInNewActivity);
        }

        private class CustomWebViewClient : WebViewClient
        {
            private readonly ProgressBar _progressBar;
            private readonly bool _openLinksInNewActivity;
            private readonly WebviewActivity _webviewActivity;

            public CustomWebViewClient(WebviewActivity webviewActivity, ProgressBar progressBar, bool openLinksInNewActivity)
            {
                _progressBar = progressBar;
                _openLinksInNewActivity = openLinksInNewActivity;
                _webviewActivity = webviewActivity;
            }

            public override void OnPageFinished(WebView view, string url)
            {
                base.OnPageFinished(view, url);
                _progressBar.Visibility = ViewStates.Gone;
            }

            public override void OnPageStarted(WebView view, string url, Bitmap favicon)
            {
                base.OnPageStarted(view, url, favicon);
                _progressBar.Visibility = ViewStates.Visible;
            }

            public override bool ShouldOverrideUrlLoading(WebView view, IWebResourceRequest request)
            {
                if (!_openLinksInNewActivity) return false;

                if (ConnectivityStatus.IsConnected)
                {
                    var intent = new Intent(Application.Context, typeof(WebviewActivity));
                    intent.PutExtra(KeyUrl, request.Url.ToString());
                    Application.Context.StartActivity(intent);
                }
                else
                {
                    _webviewActivity.ShowInfoDialog(Resource.String.Error, Resource.String.NoInternetAccess);
                }

                return true;
            }
        }
    }
}