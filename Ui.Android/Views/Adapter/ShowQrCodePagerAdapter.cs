using Android.Content;
using Android.Support.V4.App;
using Java.Lang;
using MyCC.Ui.Android.Views.Fragments;

namespace MyCC.Ui.Android.Views.Adapter
{
    public class ShowQrCodePagerAdapter : FragmentPagerAdapter
    {
        private readonly string[] _tabTitles;
        private readonly ShowQrCodeFragment[] _fragments;

        public ShowQrCodePagerAdapter(FragmentManager fm, Context context, int sourceId) : base(fm)
        {
            _tabTitles = new[]
            {
                context.Resources.GetString(Resource.String.AddressOnly),
                context.Resources.GetString(Resource.String.AllInfos)
            };

            _fragments = new[]
            {
                ShowQrCodeFragment.Create(sourceId, true),
                ShowQrCodeFragment.Create(sourceId, false)
            };
        }

        public override int Count => _fragments.Length;

        public override Fragment GetItem(int position) => _fragments[position];

        public override ICharSequence GetPageTitleFormatted(int position) => new String(_tabTitles[position]);
    }
}