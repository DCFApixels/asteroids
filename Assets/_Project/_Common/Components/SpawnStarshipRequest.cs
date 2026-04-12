using DCFApixels.DragonECS;
using UnityEngine;

namespace Asteroids.Components
{
    [System.Serializable]
    internal struct SpawnStarshipRequest : IEcsComponent
    {
        public Vector3 Position;
        public Quaternion Rotation;
    }
}