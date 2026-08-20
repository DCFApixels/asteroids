using Asteroids.StarshipsFeature;
using Asteroids.Views;
using DCFApixels;
using DCFApixels.DragonECS;
using Modules.BoundsOverlaps;
using UnityEngine;

namespace Asteroids.StarshipsFeature
{
    [CreateAssetMenu]
    public class StarshipDescription : ScriptableEntityTemplate
    {
        public UPrefab<ViewBase> ViewPrefab;
        public float BoundsRadius = 1;

        public override void Apply(short worldID, int e)
        {
            var world = EcsWorld.GetWorld(worldID);
            base.Apply(worldID, e);
            var view = (StarshipView)ViewPrefab.Spawn(null, Vector3.zero, Quaternion.identity);
            world.GetPool<ViewBase>().Set(e, view);
            world.GetPool<Transform>().Set(e, view.transform);
            ref var sphere = ref world.GetPool<BoundsSphere>().TryAddOrGet(e);
            sphere.Radius = BoundsRadius;
            ref var starship = ref world.GetPool<Starship>().TryAddOrGet(e);
            starship.View = view;
            starship.Description = this;
        }
    }
}
