using DCFApixels.DragonECS;
using UnityEngine;

namespace Asteroids.Components
{
    internal struct TransformRef : IEcsComponent
    {
        public Transform Value;
    }
}