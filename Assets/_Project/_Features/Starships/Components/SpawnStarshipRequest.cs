using DCFApixels.DragonECS;
using UnityEngine;

namespace Asteroids.StarshipsFeature
{
    [MetaGroup(StarshipsModule.META_GROUP, EcsConsts.COMPONENTS_GROUP)]
    [MetaColor(StarshipsModule.META_COLOR)]
    [System.Serializable]
    internal struct SpawnStarshipRequest : IEcsComponent
    {
        public Vector3 Position;
        public Quaternion Rotation;
    }
}
