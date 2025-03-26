using DCFApixels.DragonECS;

namespace Asteroids.Components
{
    [System.Serializable]
    public struct Asteroid : IEcsComponent
    {
        public int DeathsLeft;
        public float Radius;
    }
}