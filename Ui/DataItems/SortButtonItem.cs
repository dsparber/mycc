using System;
using MyCC.Core.Types;

namespace MyCC.Ui.DataItems
{
    public class SortButtonItem
    {
        public string Text;
        public SortDirection? SortDirection;
        public bool RightAligned;
        public Action OnClick;
    }
}