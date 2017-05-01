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
        public const string ExtraShowVersionHeader = "showVersionHeader";
        public const string ExtraOpenLinksInNewActivity = "openLinksInNewActivity";
        public const string ExtraUrl = "url";
        public const string ExtraTitle = "title";

        private bool _showVersionHeader;
        private bool _openLinksInNewActivity;

        private WebView _webView;
        private HeaderFragment _header;
        private ProgressBar _progressBar;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_webview);

            SupportActionBar.Elevation = 3;
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);

            _openLinksInNewActivity = Intent?.GetBooleanExtra(ExtraOpenLinksInNewActivity, false) ?? false;
            _showVersionHeader = Intent?.GetBooleanExtra(ExtraShowVersionHeader, false) ?? false;

            var title = Intent?.GetStringExtra(ExtraTitle);
            var url = Intent?.GetStringExtra(ExtraUrl);
            if (string.IsNullOrWhiteSpace(url)) throw new NullReferenceException("A url must be specified and passed as an extra!");

            SupportActionBar.Title = title ?? new Uri(url).Host;

            _header = (HeaderFragment)SupportFragmentManager.FindFragmentById(Resource.Id.header_fragment);
            _webView = FindViewById<WebView>(Resource.Id.webview);
            _progressBar = FindViewById<ProgressBar>(Resource.Id.progressbar);

            _header.InfoText = PackageManager.GetPackageInfo(PackageName, 0).VersionName;
            SupportFragmentManager.SetFragmentVisibility(_header, _showVersionHeader);

            _webView.SetWebViewClient(new CustomWebViewClient(_progressBar, _openLinksInNewActivity));
            _webView.LoadUrl(url);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            Finish();
            return true;
        }

        private class CustomWebViewClient : WebViewClient
        {
            private readonly ProgressBar _progressBar;
            private readonly bool _openLinksInNewActivity;

            public CustomWebViewClient(ProgressBar progressBar, bool openLinksInNewActivity)
            {
                _progressBar = progressBar;
                _openLinksInNewActivity = openLinksInNewActivity;
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

                var intent = new Intent(Application.Context, typeof(WebviewActivity));
                intent.PutExtra(ExtraUrl, request.Url.ToString());
                Application.Context.StartActivity(intent);

                return true;
            }
        }
    }
}