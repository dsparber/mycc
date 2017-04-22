namespace MyCC.Ui.Android.Data.Get
{
    public class HeaderDataItem
    {
        private readonly string _mainText;
        private readonly string _infoText;

        public virtual string MainText => _mainText;
        public virtual string InfoText => _infoText;

        public HeaderDataItem(string mainText, string infoText)
        {
            _mainText = mainText;
            _infoText = infoText;
        }
    }
}