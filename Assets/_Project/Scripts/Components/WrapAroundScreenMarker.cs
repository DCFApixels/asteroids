using DCFApixels.DragonECS;

namespace Asteroids.Components
{
    [System.Serializable]
    public struct WrapAroundScreenMarker : IEcsComponent
    {
        public float Offset;
    }
}