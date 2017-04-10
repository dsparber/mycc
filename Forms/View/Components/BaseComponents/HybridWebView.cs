using System;
using MyCC.Forms.Constants;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using MyCC.Forms.Helpers;
using Xamarin.Forms;
using System.IO;

namespace MyCC.Forms.View.Components.BaseComponents
{
    public class HybridWebView : WebView
    {
        public Action LoadFinished { set; private get; }

        private readonly Dictionary<string, Action<string>> _callbacks;

        public HybridWebView(string inputSource)
        {
            _callbacks = new Dictionary<string, Action<string>>();

            HorizontalOptions = LayoutOptions.FillAndExpand;
            VerticalOptions = LayoutOptions.FillAndExpand;
            BackgroundColor = AppConstants.BackgroundColor;
            Source = Path.Combine(DependencyService.Get<IBaseUrl>().Get(), inputSource);

            var loadedContent = false;

            Navigated += (sender, args) =>
            {
                if (loadedContent) return;
                loadedContent = true;

                LoadFinished?.Invoke();
            };

            Navigating += (sender, e) =>
            {
                if (!loadedContent) return;

                e.Cancel = true;

                var url = new Uri(e.Url);
                var args = url.Query.Substring(1, url.Query.Length - 1).Split('&').ToDictionary(s => s.Split('=')[0], s => s.Split('=')[1]);

                foreach (var a in args.Where(x => _callbacks.Keys.Contains(x.Key)))
                {
                    _callbacks[a.Key]?.Invoke(a.Value);
                }
            };
        }

        public void CallJsFunction(string function, params object[] parameter)
        {
            var args = string.Join(",", parameter.Select(JsonConvert.SerializeObject));
            Eval($"{function}({args})");
        }

        public void RegisterCallback(string key, Action<string> action)
        {
            _callbacks.Add(key, action);
        }
    }
}