using DCFApixels.DragonECS;
using UnityEngine;

namespace Asteroids.MovementFeature
{
    [MetaGroup(MovementModule.META_GROUP)]
    [MetaColor(MovementModule.META_COLOR)]
    public partial class ApplyTransformSystem : IEcsRun, IEcsDefaultAddParams
    {
        public AddParams AddParams => EcsConsts.POST_END_LAYER;

        [DI] EcsDefaultWorld _world;

        class Aspect : EcsAspect
        {
            public EcsPool<RigidTransform> RigidTransforms = Inc;
            public EcsRefPool<Transform> Transforms = Inc;
        }
        public void Run()
        {
            foreach (var e in _world.Where(out Aspect a))
            {
                ref var transform = ref a.RigidTransforms.TryAddOrGet(e);
                var goc = a.Transforms[e].transform;

                goc.position = transform.Position;
                goc.rotation = transform.Rotation;
            }
        }
    }
}