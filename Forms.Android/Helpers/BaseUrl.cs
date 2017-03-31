using MyCC.Forms.Android.Helpers;
using MyCC.Forms.Helpers;
using Xamarin.Forms;

[assembly: Dependency(typeof(BaseUrl))]
namespace MyCC.Forms.Android.Helpers
{
    public class BaseUrl : IBaseUrl
    {
        public string Get()
        {
            return "file:///android_asset/";
        }
    }
}