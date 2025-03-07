using DCFApixels.DragonECS;

namespace Asteroids.Systems
{
    internal struct InputData : IEcsComponent
    {
        public float Horizontal;
        public float Vertical;
    }
}