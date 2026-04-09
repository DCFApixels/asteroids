using Asteroids.VFX;
using Asteroids.Views;
using DCFApixels;
using DCFApixels.DragonECS;
using UnityEngine;

namespace Asteroids
{
    [CreateAssetMenu]
    public class VFXDescription : ScriptableEntityTemplate
    {
        public VFXView ViewRefab;
        public float Duration = 1;
        public (VFXView view, int entityID) Spawn(EcsWorld world, Vector3 position, Quaternion rotation)
        {
            var e = world.NewEntity();
            Apply(world.ID, e);
            var view = world.GetPool<VFXView>()[e];
            view.transform.SetPositionAndRotation(position, rotation);
            return (view, e);
        }
        public override void Apply(short worldID, int e)
        {
            var world = EcsWorld.GetWorld(worldID);
            base.Apply(worldID, e);

            if (Duration > 0)
            {
                ref var lt = ref world.GetPool<VFXLifeTime>().TryAddOrGet(e);
                lt.Duration = Duration;
                lt.Time = Duration;
            }

            var view = ViewRefab.Spawn(null);
            world.GetPool<VFXView>().Set(e, view);
        }
    }
}