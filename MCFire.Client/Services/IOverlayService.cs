using MCFire.Client.Services.Overlay;

namespace MCFire.Client.Services
{
    public interface IOverlayService
    {
        IModalOverlay Dialogue { get; }
        bool? DialogueVisible { get; }
        bool TrySetOverlay(IModalOverlay overlay);
        bool TrySetOverlay<TOverlay>() where TOverlay : IModalOverlay;
    }
}