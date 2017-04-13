using System.Collections.Generic;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.App;
using Android.Support.V4.View;
using Android.Views;
using MyCC.Ui.Android.Views.Animation;

namespace MyCC.Ui.Android.Views.Fragments
{
    public class ViewPagerFragment : Fragment
    {
        private ViewPager _viewPager;
        private PagerAdapter _pagerAdapter;

        private readonly List<Fragment> _childFragments;

        public ViewPagerFragment(List<Fragment> childFragments)
        {
            _childFragments = childFragments;
            RetainInstance = true;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.fragment_view_pager, container, false);

            _viewPager = view.FindViewById<ViewPager>(Resource.Id.pager);
            _viewPager.SetPageTransformer(true, new ZoomOutPageTransformer());

            _pagerAdapter = new ScreenSlidePagerAdapter(FragmentManager, this);
            _viewPager.Adapter = _pagerAdapter;

            var tabLayout = view.FindViewById<TabLayout>(Resource.Id.tab_dots);
            tabLayout.SetupWithViewPager(_viewPager, true);

            return view;
        }

        private class ScreenSlidePagerAdapter : FragmentStatePagerAdapter
        {
            private readonly ViewPagerFragment _parent;

            public ScreenSlidePagerAdapter(FragmentManager fm, ViewPagerFragment parent) : base(fm)
            {
                _parent = parent;
            }

            public override Fragment GetItem(int position) => _parent._childFragments[position];
            public override int Count => _parent._childFragments.Count;
        }
    }
}