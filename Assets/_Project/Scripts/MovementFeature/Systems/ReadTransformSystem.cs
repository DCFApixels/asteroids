using DCFApixels.DragonECS;

namespace Asteroids.MovementFeature
{
    [MetaGroup(nameof(MovementModule), EcsConsts.SYSTEMS_GROUP)]
    [MetaColor(MetaColor.Cyan)]
    public class ReadTransformSystem : IEcsRun, IEcsDefaultAddParams
    {
        public AddParams AddParams => EcsConsts.PRE_BEGIN_LAYER;
        class Aspect : EcsAspect
        {
            public EcsPool<GameObjectConnect> gocs = Inc;
            public EcsPool<TransformData> transforms = Inc;
        }

        [DI] EcsDefaultWorld _world;
        public void Run()
        {
            foreach (var e in _world.Where(out Aspect a))
            {
                ref var transform = ref a.transforms.Get(e);
                var goc = a.gocs.Get(e).Connect.transform;

                transform.lastPosition = transform.position;
                transform.lastRotation = transform.rotation;
                transform.position = goc.position;
                transform.rotation = goc.rotation;
            }
        }
    }
}