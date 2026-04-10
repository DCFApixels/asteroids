using DCFApixels.DragonECS;

namespace Asteroids.Components
{
    [System.Serializable]
    public struct KillRequest : IEcsComponent
    {
        public KillRequestMode Mode;
    }
    public enum KillRequestMode
    {
        Default,
        WithoutRestoring,
    }
}
