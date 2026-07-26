using DCFApixels.DragonECS;
using UnityEngine;

namespace Asteroids.StarshipsFeature
{
    [System.Serializable]
    internal struct SpawnStarshipRequest : IEcsComponent
    {
        public Vector3 Position;
        public Quaternion Rotation;
    }
}