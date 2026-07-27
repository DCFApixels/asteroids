using Asteroids.Views;
using DCFApixels.DragonECS;

namespace Asteroids.AsteroidsFeature
{
    [MetaGroup(AsteroidsModule.META_GROUP, EcsConsts.COMPONENTS_GROUP)]
    [MetaColor(AsteroidsModule.META_COLOR)]
    [System.Serializable]
    public struct Asteroid : IEcsComponent
    {
        public AsteroidDescription Description;
        public AsteroidView View;
        public int DeathsLeft;
    }
}
