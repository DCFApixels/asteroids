using DCFApixels;
using DCFApixels.DragonECS;
using DCFApixels.DragonECS.Core;
using UnityEngine;

namespace Modules.FX
{
    [System.Serializable]
    internal struct FX : IEcsComponent, IEcsComponentLifecycle<FX>
    {
        public Component PooledInstance;
        public UPool Pool;
        void IEcsComponentLifecycle<FX>.OnAdd(ref FX component, short worldID, int entityID) { }
        void IEcsComponentLifecycle<FX>.OnDel(ref FX component, short worldID, int entityID)
        {
            component.Pool.DespawnRaw(component.PooledInstance);
            component = default;
        }
    }
}