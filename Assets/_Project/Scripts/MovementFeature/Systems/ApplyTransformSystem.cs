using DCFApixels.DragonECS;

namespace Asteroids.MovementFeature
{
    [MetaGroup(nameof(MovementModule), EcsConsts.SYSTEMS_GROUP)]
    [MetaColor(MetaColor.Cyan)]
    public partial class ApplyTransformSystem : IEcsRun, IEcsDefaultAddParams
    {
        public AddParams AddParams => EcsConsts.POST_END_LAYER;
        class Aspect : EcsAspect
        {
            public EcsPool<TransformData> transforms = Inc;
            public EcsPool<GameObjectConnect> gocs = Inc;
        }

        [DI] EcsDefaultWorld _world;

        public void Run()
        {
            foreach (var e in _world.Where(out Aspect a))
            {
                ref var transform = ref a.transforms.TryAddOrGet(e);
                var goc = a.gocs.Get(e).Connect.transform;
                 
                goc.position = transform.position;
                goc.rotation = transform.rotation;
            }
        }
    }
}