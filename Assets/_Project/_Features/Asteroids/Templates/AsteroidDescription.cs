using Asteroids.Components;
using Asteroids.Views;
using DCFApixels;
using DCFApixels.DragonECS;
using Modules.BoundsOverlaps;
using UnityEngine;

namespace Asteroids.AsteroidsFeature
{
    [CreateAssetMenu]
    public class AsteroidDescription : ScriptableEntityTemplate
    {
        public UPrefab<ViewBase> ViewPrefab;
        public float BoundsRadius = 1;
        public int DeathsCount = 2;

        public override void Apply(short worldID, int e)
        {
            var world = EcsWorld.GetWorld(worldID);
            base.Apply(worldID, e);
            var view = (AsteroidView)ViewPrefab.Spawn(null, Vector3.zero, Quaternion.identity);
            world.GetPool<ViewBase>().Set(e, view);
            world.GetPool<Transform>().Set(e, view.transform);
            view.SetRadius(BoundsRadius);
            ref var sphere = ref world.GetPool<BoundsSphere>().TryAddOrGet(e);
            sphere.Radius = BoundsRadius;
            ref var asteroid = ref world.GetPool<Asteroid>().TryAddOrGet(e);
            asteroid.DeathsLeft = DeathsCount;
            asteroid.View = view;
            asteroid.Description = this;
        }
    }
}
