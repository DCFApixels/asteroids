using Asteroids.BulletsFeature;
using Asteroids.Views;
using DCFApixels;
using DCFApixels.DragonECS;
using Modules.BoundsOverlaps;
using UnityEngine;

namespace Asteroids.BulletsFeature
{
    [CreateAssetMenu]
    public class BulletDescription : ScriptableEntityTemplate
    {
        public ProjectileView ViewPrefab;
        public float BoundsRadius = 1;
        public float LifeTime = 3;

        public override void Apply(short worldID, int e)
        {
            var world = EcsWorld.GetWorld(worldID);
            base.Apply(worldID, e);
            var viewRaw = ViewPrefab.Spawn<ViewBase>(null, Vector3.zero, Quaternion.identity);
            var view = (ProjectileView)viewRaw;
            world.GetPool<ViewBase>().Set(e, view);
            world.GetPool<Transform>().Set(e, view.transform);
            ref var sphere = ref world.GetPool<BoundsSphere>().TryAddOrGet(e);
            sphere.Radius = BoundsRadius;
            ref var projectile = ref world.GetPool<Bullet>().TryAddOrGet(e);
            projectile.Description = this;
            ref var lifeTime = ref world.GetPool<BulletLifeTime>().TryAddOrGet(e);
            lifeTime.Time = LifeTime;
        }
    }
}