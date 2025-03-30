using DCFApixels.DragonECS;

namespace Asteroids.Components
{
    [System.Serializable]
    internal struct PooledUnit : IEcsComponent
    {
        public UnityEngine.Component Unit;
        public int ID;
    }
}