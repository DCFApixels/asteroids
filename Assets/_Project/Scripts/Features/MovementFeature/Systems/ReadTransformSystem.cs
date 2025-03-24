using DCFApixels.DragonECS;

namespace Asteroids.MovementFeature
{
    [MetaGroup(MovementModule.META_GROUP)]
    [MetaColor(MovementModule.META_COLOR)]
    public class ReadTransformSystem : IEcsRun, IEcsDefaultAddParams
    {
        public AddParams AddParams => EcsConsts.PRE_BEGIN_LAYER;
        class Aspect : EcsAspect
        {
            public EcsPool<GameObjectConnect> connects = Inc;
            public EcsPool<TransformData> transforms = Inc;
        }
        [DI] EcsDefaultWorld _world;
        public void Run()
        {
            foreach (var e in _world.Where(out Aspect a))
            {
                ref var transform = ref a.transforms.Get(e);
                var connect = a.connects.Get(e).Connect.transform;

                transform.lastPosition = transform.position;
                transform.lastRotation = transform.rotation;
                transform.position = connect.position;
                transform.rotation = connect.rotation;
            }
        }
    }
}