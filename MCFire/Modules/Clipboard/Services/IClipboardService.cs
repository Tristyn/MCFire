namespace MCFire.Modules.Clipboard.Services
{
    public interface IClipboardService
    {
        object Data { get; }
    }

    public class ClipboardCopyEvent
    {
        public ClipboardCopyEvent(object data)
        {
            Data = data;
        }

        public object Data { get; private set; }
    }
}
