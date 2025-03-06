using Asteroids.Views;
using DCFApixels.DragonECS;

namespace Asteroids.Components
{
    internal struct Asteroid : IEcsComponent
    {
        public AsteroidView View;

        public int DeathsLeft;
        public float Radius;
    }
}