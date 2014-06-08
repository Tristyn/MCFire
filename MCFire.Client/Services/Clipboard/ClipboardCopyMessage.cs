namespace MCFire.Client.Services.Clipboard
{
    public class ClipboardCopyMessage
    {
        public ClipboardCopyMessage(object data)
        {
            Data = data;
        }

        public object Data { get; private set; }
    }
}
