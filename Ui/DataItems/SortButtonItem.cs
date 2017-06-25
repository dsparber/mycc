using System;

namespace MyCC.Ui.DataItems
{
    public class SortButtonItem
    {
        public string Text { get; internal set; }
        public bool? SortAscending { get; internal set; }
        public bool RightAligned { get; internal set; }
        public Action OnClick { get; internal set; }
    }
}