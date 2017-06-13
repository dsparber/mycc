namespace MyCC.Ui.DataItems
{
    public class HeaderItem
    {
        public virtual string MainText { get; }

        public virtual string InfoText { get; }

        public HeaderItem(string mainText, string infoText)
        {
            MainText = mainText;
            InfoText = infoText;
        }
    }
}