using DCFApixels;
using DCFApixels.DragonECS;
using DCFApixels.DragonECS.Core;
using UnityEngine;
using Modules.FX;

namespace Asteroids.Views
{
    public class ViewBase : MonoBehaviour, IUPoolUnit<ViewBase>, IEcsComponentLifecycle<ViewBase>
    {
        [SerializeField]
        private int _preWarmCount = 8;
        [SerializeField]
        private entlong _entity;

        public ShortVFXView DeathVFX;

        public virtual float GetScale()
        {
            return transform.localScale.x;
        }



        private UPool _sourcePool;
        public void Despawn()
        {
            _sourcePool?.DespawnRaw(this);
        }
        void IUPoolUnit<ViewBase>.Static_InitPool(UPool pool)
        {
            pool.Prewarm(_preWarmCount);
        }
        void IUPoolUnit<ViewBase>.Static_InitPoolUnit(ViewBase self, UPool pool)
        {
            self._sourcePool = pool;
        }
        void IUPoolUnit<ViewBase>.Static_ResetUnit(ViewBase self, ViewBase prefab)
        {
            // The pool reference is stable for the instance lifetime and is needed after respawn.
        }
        void IEcsComponentLifecycle<ViewBase>.OnAdd(ref ViewBase component, short worldID, int entityID)
        {
            component._entity = (EcsWorld.GetWorld(worldID), entityID);
        }
        void IEcsComponentLifecycle<ViewBase>.OnDel(ref ViewBase component, short worldID, int entityID)
        {
            component.Despawn();
            component._entity = entlong.NULL;
        }
    }
}
