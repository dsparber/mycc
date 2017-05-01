using System;
using Android.OS;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using MyCC.Core.Types;
using MyCC.Ui.Android.Helpers;
using MyCC.Ui.DataItems;

namespace MyCC.Ui.Android.Views.Fragments
{
    public class SortButtonFragment : Fragment
    {
        public SortButtonItem Data
        {
            set
            {
                Text = value.Text;
                Direction = value.SortDirection;
                Gravity = value.RightAligned ? GravityFlags.Right : GravityFlags.Left;
                OnClick = value.OnClick;
            }
        }

        private string Text
        {
            set { _textView.Text = value; }
        }

        public bool First
        {
            set { _textView.SetPadding((value ? 24 : 8).DpToPx(), 8.DpToPx(), 4.DpToPx(), 8.DpToPx()); }
        }
        public bool Last
        {
            set { _imageView.SetPadding(0, 0, (value ? 24 : 8).DpToPx(), 0); }
        }

        private SortDirection? Direction
        {
            set
            {
                switch (value)
                {
                    case null: _imageView.SetImageResource(Resource.Drawable.ic_sort); break;
                    case SortDirection.Ascending: _imageView.SetImageResource(Resource.Drawable.ic_sort_asc); break;
                    case SortDirection.Descending: _imageView.SetImageResource(Resource.Drawable.ic_sort_desc); break;
                }
            }
        }

        private GravityFlags Gravity
        {
            set { _textView.Gravity = value; }
        }

        private Action OnClick { get; set; }

        private TextView _textView;
        private ImageView _imageView;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.fragment_sort_button, container, false);
            view.SetOnClickListener(new OnClickListener(this));

            _textView = view.FindViewById<TextView>(Resource.Id.text);
            _imageView = view.FindViewById<ImageView>(Resource.Id.image);

            return view;
        }

        private class OnClickListener : Java.Lang.Object, View.IOnClickListener
        {
            private readonly SortButtonFragment _parent;

            public OnClickListener(SortButtonFragment parent)
            {
                _parent = parent;
            }

            public void OnClick(View v)
            {
                _parent.OnClick?.Invoke();
            }
        }
    }
}