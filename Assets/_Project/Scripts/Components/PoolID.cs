using DCFApixels.DragonECS;
using UnityEngine;

namespace Asteroids.Components
{
    public struct PoolID : IEcsComponent
    {
        public Component Component;
        public int Id;
    }
}