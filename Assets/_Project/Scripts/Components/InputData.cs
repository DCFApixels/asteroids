using DCFApixels.DragonECS;
using System.Numerics;

namespace Asteroids.Components
{
    internal struct InputData : IEcsComponent
    {
        public float Horizontal;
        public float Vertical;
        public Vector2 Axis;
    }
}