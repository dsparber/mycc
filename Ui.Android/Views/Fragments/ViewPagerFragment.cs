using System.Collections.Generic;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.App;
using Android.Support.V4.View;
using Android.Views;
using MyCC.Ui.Android.Views.Animation;
using System;

namespace MyCC.Ui.Android.Views.Fragments
{
    public class ViewPagerFragment : Fragment
    {
        private ViewPager _viewPager;
        private PagerAdapter _pagerAdapter;

        private readonly List<Fragment> _childFragments;
        private readonly int _startPosition;

        public ViewPagerFragment(List<Fragment> childFragments, int startPosition)
        {
            _childFragments = childFragments;
            _startPosition = startPosition;
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

            Position = _startPosition;
            _viewPager.AddOnPageChangeListener(new PageChangedListener(this));

            return view;
        }

        public int Position
        {
            set
            {
                if (value >= 0 && value < (_viewPager?.Adapter.Count ?? 0))
                {
                    _viewPager?.SetCurrentItem(value, false);
                }
            }
        }

        public Action<int> PositionChanged { set; private get; }

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

        private class PageChangedListener : Java.Lang.Object, ViewPager.IOnPageChangeListener
        {
            private ViewPagerFragment _parent;

            public PageChangedListener(ViewPagerFragment parent)
            {
                _parent = parent;
            }

            public void OnPageScrolled(int position, float positionOffset, int positionOffsetPixels)
            {
                /* Nothing */
            }

            public void OnPageScrollStateChanged(int state)
            {
                /* Nothing */
            }

            public void OnPageSelected(int position)
            {
                _parent.PositionChanged?.Invoke(position);
            }
        }
    }
}