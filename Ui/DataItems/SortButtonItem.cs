using System;

namespace MyCC.Ui.DataItems
{
    public class SortButtonItem
    {
        public string Text;
        public bool? SortAscending;
        public bool RightAligned;
        public Action OnClick;
    }
}