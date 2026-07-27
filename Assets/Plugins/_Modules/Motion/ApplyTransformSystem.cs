using DCFApixels.DragonECS;
using UnityEngine;

namespace Modules.Motion
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
                ref var rigidTransform = ref a.RigidTransforms.TryAddOrGet(e);
                var transform = a.Transforms[e].transform;

                rigidTransform.Rotation = rigidTransform.Rotation.normalized;
                for (int i = 0; i < 4; i++)
                {
                    var v = rigidTransform.Rotation[i];
                    if (float.IsNaN(v))
                    {
                        rigidTransform.Rotation = Quaternion.identity;
                        break;
                    }
                }
                for (int i = 0; i < 3; i++)
                {
                    var v = rigidTransform.Position[i];
                    if (float.IsNaN(v))
                    {
                        rigidTransform.Position[i] = 0;
                    }
                }

                transform.SetPositionAndRotation(rigidTransform.Position, rigidTransform.Rotation);
            }
        }
    }
}