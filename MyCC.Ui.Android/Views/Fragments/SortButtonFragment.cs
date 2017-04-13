using System;
using Android.OS;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using MyCC.Core.Types;

namespace MyCC.Ui.Android.Views.Fragments
{
    public class SortButtonFragment : Fragment
    {
        public string Text
        {
            get { return _textView.Text; }
            set { _textView.Text = value; }
        }

        private SortDirection? _direction;

        public SortDirection? Direction
        {
            get { return _direction; }
            set
            {
                _direction = value;
                switch (value)
                {
                    case null: _imageView.SetImageResource(Resource.Drawable.ic_sort); break;
                    case SortDirection.Ascending: _imageView.SetImageResource(Resource.Drawable.ic_sort_asc); break;
                    case SortDirection.Descending: _imageView.SetImageResource(Resource.Drawable.ic_sort_desc); break;
                }
            }
        }

        public GravityFlags Gravity
        {
            get { return _textView.Gravity; }
            set { _textView.Gravity = value; }
        }

        public Action OnClick { private get; set; }

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