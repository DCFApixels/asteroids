using DCFApixels.DragonECS;
using Modules.Movement;
using System.Linq;
using UnityEngine;

namespace Modules.CameraController
{
    [MetaGroup(CameraControllerModule.META_GROUP)]
    [MetaColor(CameraControllerModule.META_COLOR)]
    internal class CameraSmoothFollowSystem : IEcsRun, IEcsDefaultAddParams
    {
        public AddParams AddParams => EcsConsts.POST_END_LAYER;

        [DI] EcsDefaultWorld _world;
        [DI] CameraBrain _brain;

        class Aspect : EcsAspect
        {
            public EcsPool<RigidTransform> TransformDatas = Inc;
            public EcsPool<CameraSmoothFollowTarget> cameraSmoothFollowTargets = Inc;
        }
        public void Run()
        {
            foreach (var e in _world.Where(out Aspect a))
            {
                ref var transformData = ref a.TransformDatas[e];
                ref var cameraSmoothFollowTarget = ref a.cameraSmoothFollowTargets[e];

                Vector3 campos = Vector3.Lerp(To2D(cameraSmoothFollowTarget.Target), To2D(transformData.Position), cameraSmoothFollowTarget.PositionsLerp);
                campos.y = _brain.Pivod.position.y;

                campos = Vector3.Lerp(_brain.Pivod.position, campos, cameraSmoothFollowTarget.MoveLerp);
                _brain.Pivod.position = campos;
            }
        }
        private static Vector3 To2D(Vector3 v)
        {
            v.y = 0;
            return v;
        }
    }
}
