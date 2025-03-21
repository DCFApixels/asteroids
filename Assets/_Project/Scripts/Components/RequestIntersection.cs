using DCFApixels.DragonECS;

namespace Asteroids.Components
{
    internal struct RequestIntersection : IEcsComponent
    {
        public float CheckRadius;
        public float ObjectRadius;
    }
}