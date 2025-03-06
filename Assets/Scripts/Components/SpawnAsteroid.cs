using DCFApixels.DragonECS;
using UnityEngine;

namespace Asteroids.Components
{
    internal struct SpawnAsteroid : IEcsComponent
    {
        public int DeathsLeft;
        public float StartRadius;
        public Vector3 Position;
        public Quaternion Rotation;
    }
}