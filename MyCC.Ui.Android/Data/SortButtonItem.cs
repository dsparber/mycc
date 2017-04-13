using System;
using Android.Views;
using MyCC.Core.Types;

namespace MyCC.Ui.Android.Data
{
    public class SortButtonItem
    {
        public string Text;
        public SortDirection? SortDirection;
        public GravityFlags TextGravity;
        public Action OnClick;
    }
}