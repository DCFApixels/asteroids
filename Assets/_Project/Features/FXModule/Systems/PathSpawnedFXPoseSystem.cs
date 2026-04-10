using DCFApixels.DragonECS;
using UnityEngine;

namespace Modules.FX
{
    [MetaGroup(FXModule.META_GROUP)]
    [MetaColor(FXModule.META_COLOR)]
    public class PathSpawnedFXPoseSystem : IEcsRun
    {
        [DI] EcsDefaultWorld _world;
        class Aspect : EcsAspect
        {
            public EcsPool<FXSpawnedEvent> SpawnedEvent = Inc;
            public EcsPool<SpawnFXRequestPose> Requests = Inc;
            public EcsRefPool<Transform> Transforms = Inc;
        }
        public void Run()
        {
            _world.GetAspects(out Aspect a);
            foreach (var reqE in _world.Where(a))
            {
                ref var req = ref a.Requests[reqE];
                var transform = a.Transforms[reqE];

                transform.SetPositionAndRotation(req.Position, req.Rotation);
            }

            a.Requests.ClearAll();
        }
    }
}
