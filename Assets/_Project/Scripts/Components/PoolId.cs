using DCFApixels.DragonECS;
using UnityEngine;

namespace Asteroids.Components
{
    internal struct PoolId : IEcsComponent
    {
        public Component Component;
        public int Id;
    }
}