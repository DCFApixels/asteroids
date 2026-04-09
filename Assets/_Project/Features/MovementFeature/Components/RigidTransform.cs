using DCFApixels.DragonECS;
using UnityEngine;

namespace Modules.Movement
{
    [MetaGroup(MovementModule.META_GROUP)]
    [MetaColor(MovementModule.META_COLOR)]
    [MetaID("E9522BB9950115AD62A48B42DEA0B5ED")]
    [System.Serializable]
    public struct RigidTransform : IEcsComponent
    {
        public static readonly RigidTransform Default = new RigidTransform()
        {
            Position = default,
            Rotation = Quaternion.identity,
            LastPosition = default,
            LastRotation = Quaternion.identity,
        };

        [Header("Current")]
        public Vector3 Position;
        public Quaternion Rotation;

        [Header("Last")]
        public Vector3 LastPosition;
        public Quaternion LastRotation;

        public Vector3 ToLocalVector(Vector3 vector)
        {
            return Rotation * vector;
        }
    }
}
