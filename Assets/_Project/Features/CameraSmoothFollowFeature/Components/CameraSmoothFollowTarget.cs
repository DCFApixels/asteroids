using DCFApixels.DragonECS;
using UnityEngine;

namespace Asteroids.CameraSmoothFollowFeature
{
    [MetaGroup(CameraSmoothFollowModule.META_GROUP)]
    [MetaColor(CameraSmoothFollowModule.META_COLOR)]
    [System.Serializable]
    public struct CameraSmoothFollowTarget : IEcsComponent
    {
        public Vector3 target;
        public float positionsLerp;
        public float moveLerp;
    }
}
