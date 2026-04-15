using DCFApixels.DragonECS;
using UnityEngine;

namespace Asteroids.Components
{
    internal struct SpawnAsteroidRequest : IEcsComponent
    {
        public AsteroidDescription Description;
        public float OverrideRadius;
        public int OverrideDeathsCount;
        public Vector3 Position;
        public Quaternion Rotation;
    }
}