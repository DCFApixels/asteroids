using DCFApixels.DragonECS;
using UnityEngine;

namespace Modules.Movement
{
    [MetaGroup(MovementModule.META_GROUP)]
    [MetaColor(MovementModule.META_COLOR)]
    public class ReadTransformSystem : IEcsRun, IEcsDefaultAddParams
    {
        public AddParams AddParams => EcsConsts.PRE_BEGIN_LAYER;

        [DI] EcsDefaultWorld _world;

        class Aspect : EcsAspect
        {
            public EcsRefPool<Transform> Transforms = Inc;
            public EcsPool<RigidTransform> RigidTransforms = Inc;
        }
        public void Run()
        {
            foreach (var e in _world.Where(out Aspect a))
            {
                ref var rigidTransform = ref a.RigidTransforms[e];
                var transform = a.Transforms[e];

                rigidTransform.LastPosition = rigidTransform.Position;
                rigidTransform.LastRotation = rigidTransform.Rotation;
                rigidTransform.Position = transform.position;
                rigidTransform.Rotation = transform.rotation;
            }
        }
    }
}