using DCFApixels;
using DCFApixels.DragonECS;
using UnityEngine;

namespace Modules.FX
{
    [CreateAssetMenu]
    public class VFXDescription : ScriptableEntityTemplate
    {
        public ShortVFXView ViewRefab;
        public float Duration => ViewRefab.Duration;
        public (ShortVFXView view, int entityID) Spawn(EcsWorld world, Vector3 position, Quaternion rotation)
        {
            var e = world.NewEntity();
            Apply(world.ID, e);
            var view = world.GetPool<FX>()[e].PooledInstance;  
            view.transform.SetPositionAndRotation(position, rotation);
            return ((ShortVFXView)view, e);
        }
        public override void Apply(short worldID, int e)
        {
            var world = EcsWorld.GetWorld(worldID);
            base.Apply(worldID, e);

            if (Duration > 0)
            {
                ref var lt = ref world.GetPool<FXLifeTime>().TryAddOrGet(e);
                lt.Duration = Duration;
                lt.Time = Duration;
            }

            ref var fx = ref world.GetPool<FX>().TryAddOrGet(e);

            var pool = UPool.GetFor(ViewRefab);
            fx.Pool = pool;
            fx.PooledInstance = pool.Spawn(null, Vector3.zero, Quaternion.identity);
        }
    }
}