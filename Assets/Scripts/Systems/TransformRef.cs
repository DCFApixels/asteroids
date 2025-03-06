using DCFApixels.DragonECS;
using UnityEngine;

namespace Asteroids.Systems
{
    internal struct TransformRef : IEcsComponent
    {
        public Transform Value;
    }
}