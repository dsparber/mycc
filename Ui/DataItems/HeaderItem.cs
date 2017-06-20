namespace MyCC.Ui.DataItems
{
    public class HeaderItem
    {
        public string MainText { get; internal set; }

        public string InfoText { get; internal set; }

        public HeaderItem(string mainText, string infoText)
        {
            MainText = mainText;
            InfoText = infoText;
        }
    }
}