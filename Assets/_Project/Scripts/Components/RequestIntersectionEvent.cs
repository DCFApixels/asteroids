using DCFApixels.DragonECS;

namespace Asteroids.Components
{
    internal struct RequestIntersectionEvent : IEcsComponent
    {
        public float CheckRadius;
        public float ObjectRadius;
    }
}