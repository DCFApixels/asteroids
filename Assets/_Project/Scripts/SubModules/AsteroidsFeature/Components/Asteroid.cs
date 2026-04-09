using Asteroids.Views;
using DCFApixels.DragonECS;

namespace Asteroids.Components
{
    [System.Serializable]
    public struct Asteroid : IEcsComponent
    {
        public AsteroidDescription Description;
        public AsteroidView View;
        public int DeathsLeft;
    }
}