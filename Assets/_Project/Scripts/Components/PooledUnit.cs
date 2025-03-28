using DCFApixels.DragonECS;

namespace Asteroids.Components
{
    internal struct PooledUnit : IEcsComponent
    {
        public UnityEngine.Component Unit;
        public int ID;
    }
}