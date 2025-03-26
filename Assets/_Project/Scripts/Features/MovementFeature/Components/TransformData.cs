using DCFApixels.DragonECS;
using UnityEngine;

namespace Asteroids.MovementFeature
{
    [System.Serializable]
    [MetaGroup(MovementModule.META_GROUP)]
    [MetaColor(MovementModule.META_COLOR)]
    [MetaID("E9522BB9950115AD62A48B42DEA0B5ED")]
    public struct TransformData : IEcsComponent
    {
        public static readonly TransformData Default = new TransformData()
        {
            position = default,
            rotation = Quaternion.identity,
            lastPosition = default,
            lastRotation = Quaternion.identity,
        };

        [Header("Current")]
        public Vector3 position;
        public Quaternion rotation;

        [Header("Last")]
        public Vector3 lastPosition;
        public Quaternion lastRotation;

        public Vector3 CalcLocalVector(Vector3 vector)
        {
            return rotation * vector;
        }
    }
}
