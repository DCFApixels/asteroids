using DCFApixels.DragonECS;

namespace Asteroids.Components
{
    internal struct Asteroid : IEcsComponent
    {
        public int DeathsLeft;
        public float Radius;
    }
}