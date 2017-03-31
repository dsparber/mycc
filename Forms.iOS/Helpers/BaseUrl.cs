using System;
using MyCC.Forms.Helpers;
using MyCC.Forms.iOS.Helpers;
using Xamarin.Forms;

[assembly: Dependency(typeof(BaseUrl))]

namespace MyCC.Forms.iOS.Helpers
{
    public class BaseUrl : IBaseUrl
    {
        public string Get()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.Personal).Replace("Documents", "tmp/");
        }
    }
}