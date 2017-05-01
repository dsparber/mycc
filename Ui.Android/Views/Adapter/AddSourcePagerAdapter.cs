using Android.Content;
using Android.Support.V4.App;
using Java.Lang;
using MyCC.Ui.Android.Views.Fragments.AddSource;

namespace MyCC.Ui.Android.Views.Adapter
{
    public class AddSourcePagerAdapter : FragmentPagerAdapter
    {
        private readonly string[] _tabTitles;
        private readonly AddSourceFragment[] _fragments;

        public AddSourcePagerAdapter(FragmentManager fm, Context context) : base(fm)
        {
            _tabTitles = new[]
            {
                context.Resources.GetString(Resource.String.Address),
                context.Resources.GetString(Resource.String.Bittrex),
                context.Resources.GetString(Resource.String.Manually),
            };

            _fragments = new AddSourceFragment[]
            {
                new AddAddressFragment(),
                new AddBittrexFragment(),
                new AddManuallyFragment()
            };
        }

        public override int Count => 3;

        public override Fragment GetItem(int position)
        {
            return _fragments[position];
        }


        public AddSourceFragment GetFragment(int position)
        {
            return _fragments[position];
        }

        public override ICharSequence GetPageTitleFormatted(int position)
        {
            return new String(_tabTitles[position]);
        }
    }
}