using DCFApixels.DragonECS;

namespace Modules.Motion
{
    [MetaGroup(MovementModule.META_GROUP)]
    [MetaColor(MovementModule.META_COLOR)]
    public class RigidTransformSnapshotSystem : IEcsRun, IEcsDefaultAddParams
    {
        public AddParams AddParams => EcsConsts.PRE_BEGIN_LAYER;

        [DI] EcsDefaultWorld _world;

        class Aspect : EcsAspect
        {
            public EcsPool<RigidTransform> RigidTransforms = Inc;
        }
        public void Run()
        {
            foreach (var e in _world.Where(out Aspect a))
            {
                ref var rigidTransform = ref a.RigidTransforms[e];

                rigidTransform.LastPosition = rigidTransform.Position;
                rigidTransform.LastRotation = rigidTransform.Rotation;
            }
        }
    }
}