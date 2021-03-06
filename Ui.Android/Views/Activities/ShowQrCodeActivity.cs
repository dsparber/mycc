﻿using System;
using System.Linq;
using Android.App;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Android.Views;
using MyCC.Core.Account.Repositories.Base;
using MyCC.Core.Account.Storage;
using MyCC.Ui.Android.Helpers;
using MyCC.Ui.Android.Views.Adapter;
using MyCC.Ui.Android.Views.Fragments;

namespace MyCC.Ui.Android.Views.Activities
{
    [Activity(Theme = "@style/MyCC", Label = "@string/QrCode")]
    public class ShowQrCodeActivity : MyccActivity
    {
        private ShowQrCodePagerAdapter _pagerAdapter;
        private ViewPager _viewPager;
        private int _sourceId;

        public const string KeySourceId = "SourceId";

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            bundle = bundle ?? new Bundle();
            SetContentView(Resource.Layout.activity_show_qr_code);

            _sourceId = bundle.GetInt(KeySourceId, Intent?.GetIntExtra(KeySourceId, -1) ?? -1);
            var source = AccountStorage.Instance.Repositories.OfType<AddressAccountRepository>().FirstOrDefault(r => r.Id == _sourceId);
            if (source == null) throw new NullReferenceException("A source id needs to be specified and passed with the intent!");

            _viewPager = FindViewById<ViewPager>(Resource.Id.viewpager);
            _pagerAdapter = new ShowQrCodePagerAdapter(SupportFragmentManager, this, _sourceId);
            _viewPager.Adapter = _pagerAdapter;

            var tabLayout = FindViewById<TabLayout>(Resource.Id.sliding_tabs);
            tabLayout.SetupWithViewPager(_viewPager);

            var header = (HeaderFragment)SupportFragmentManager.FindFragmentById(Resource.Id.header_fragment);
            header.MainText = source.Currency.Code;
            header.InfoText = source.Name;

            var activityRootView = FindViewById(Resource.Id.view_root);
            activityRootView.ViewTreeObserver.GlobalLayout += (sender, args) => SupportFragmentManager.SetFragmentVisibility(header, activityRootView.Height > 480.DpToPx());
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            Finish();
            return true;
        }

        protected override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);
            outState.PutInt(KeySourceId, _sourceId);
        }
    }
}