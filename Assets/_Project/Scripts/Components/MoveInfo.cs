using DCFApixels.DragonECS;
using System;
using Vector3 = UnityEngine.Vector3;

namespace Asteroids.Components
{
    [Serializable]
    internal struct MoveInfo : IEcsComponent
    {
        public Vector3 Position;
        public Vector3 Forward;
        public float Speed;
        public float Acceleration;
        public float RotationSpeed;
        public float Power;
        public float Friction;

        public float DefaultRotationSpeed;
        public float MaxSpeed;
    }
}