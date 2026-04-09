using DCFApixels.DragonECS;
using UnityEngine;

namespace Asteroids.GameFieldFueature
{
    [System.Serializable]
    [MetaGroup(GameFieldModule.META_GROUP)]
    [MetaColor(GameFieldModule.META_COLOR)]
    internal struct OutOfGameFieldEvent : IEcsComponent
    {
        public Vector3 Excess;
    }
}
