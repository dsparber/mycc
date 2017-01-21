namespace MyCC.Forms.Messages
{
    public class MessageInfo
    {
        private readonly string action;

        private MessageInfo(string action)
        {
            this.action = action;
        }

        public static readonly MessageInfo Started = new MessageInfo("started");
        public static readonly MessageInfo Finished = new MessageInfo("finished");
        public static readonly MessageInfo ValueChanged = new MessageInfo("valueChanged");


        public bool IsStarted => Equals(Started);
        public bool IsFinished => Equals(Finished);
        public bool IsValueChanged => Equals(ValueChanged);

        public override bool Equals(object obj) => (obj as MessageInfo)?.action.Equals(action) ?? false;

        public override int GetHashCode() => action.GetHashCode();

        public override string ToString() => action;
    }
}

