using DCFApixels.DragonECS;
using UnityEngine;

namespace Asteroids.Components
{
    internal struct PoolID : IEcsComponent
    {
        public Component Component;
        public int Id;
    }
}