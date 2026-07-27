using DCFApixels.DragonECS;
using UnityEngine;

namespace Asteroids.AsteroidsFeature
{
    [MetaGroup(AsteroidsModule.META_GROUP, EcsConsts.COMPONENTS_GROUP)]
    [MetaColor(AsteroidsModule.META_COLOR)]
    internal struct SpawnAsteroidRequest : IEcsComponent
    {
        public AsteroidDescription Description;
        public float OverrideRadius;
        public int OverrideDeathsCount;
        public Vector3 Position;
        public Quaternion Rotation;
    }
}
