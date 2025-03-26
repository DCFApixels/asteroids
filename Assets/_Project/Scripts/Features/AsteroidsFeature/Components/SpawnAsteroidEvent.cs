using DCFApixels.DragonECS;
using UnityEngine;

namespace Asteroids.Components
{
    internal struct SpawnAsteroidEvent : IEcsComponent
    {
        public int DeathsLeft;
        public float StartRadius;
        public Vector3 Position;
        public Quaternion Rotation;
    }
}