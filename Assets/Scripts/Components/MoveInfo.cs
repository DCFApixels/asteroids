using DCFApixels.DragonECS;
using Vector3 = UnityEngine.Vector3;

namespace Asteroids.Components
{
    internal struct MoveInfo : IEcsComponent
    {
        public Vector3 Position;
        public Vector3 Forward;
        public float Speed;
        public float Acceleration;
        public float RotationSpeed;

        public float DefaultRotationSpeed;
        public float DefaultSpeed;
    }
}