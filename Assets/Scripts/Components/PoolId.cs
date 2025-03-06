using DCFApixels.DragonECS;
using UnityEngine;

namespace Asteroids.Components
{
    public struct PoolId : IEcsComponent
    {
        public Component Component;
        public int Id;
    }
}