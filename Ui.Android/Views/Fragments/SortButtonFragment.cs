using System;
using Android.OS;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
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
                try
                {
                    Text = value.Text;
                    Ascending = value.SortAscending;
                    Gravity = value.RightAligned ? GravityFlags.Right : GravityFlags.Left;
                    OnClick = value.OnClick;
                }
                catch {/* not attached */}
            }
        }

        private string Text
        {
            set
            {
                try
                {
                    _textView.Text = value;
                }
                catch {/* not attached */}
            }
        }

        public bool First
        {
            set
            {
                try
                {
                    _textView.SetPadding(value ? Resources.GetDimensionPixelSize(Resource.Dimension.abc_action_bar_content_inset_material) : 8.DpToPx(), 8.DpToPx(), 4.DpToPx(), 8.DpToPx());
                }
                catch {/* not attached */}
            }
        }

        public bool Last
        {
            set
            {
                try
                {
                    _imageView.SetPadding(0, 0, value ? Resources.GetDimensionPixelSize(Resource.Dimension.abc_action_bar_content_inset_material) : 8.DpToPx(), 0);
                }
                catch {/* not attached */}
            }
        }

        private bool? Ascending
        {
            set
            {
                switch (value)
                {
                    case null: _imageView.SetImageResource(Resource.Drawable.ic_sort); break;
                    case true: _imageView.SetImageResource(Resource.Drawable.ic_sort_asc); break;
                    case false: _imageView.SetImageResource(Resource.Drawable.ic_sort_desc); break;
                }
            }
        }

        private GravityFlags Gravity
        {
            set => _textView.Gravity = value;
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