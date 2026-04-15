using DCFApixels;
using DCFApixels.DragonECS;
using UnityEngine;

namespace Modules.FX
{
    [MetaGroup(FXModule.META_GROUP)]
    [MetaColor(FXModule.META_COLOR)]
    public class SpawnFXSystem : IEcsRun
    {
        [DI] EcsDefaultWorld _world;
        class RequestAspect : EcsAspect
        {
            public EcsPool<ShortVFXSpawnRequest> Requests = Inc;
            public EcsPool<FXSpawnedEvent> Events = Opt;
            public EcsPool<FXLifeTime> LifeTimes = Opt;
            public EcsPool<FX> FXs = Opt;
        }
        public void Run()
        {
            _world.GetAspects(out RequestAspect reqA);
            foreach (var reqE in _world.Where(reqA))
            {
                ref var req = ref reqA.Requests[reqE];
                var pool = UPool.GetFor(req.Prefab);

                Quaternion rot;
                if(req.Direction == null || req.Direction == Vector3.zero)
                {
                    rot = req.Rotation;
                }
                else
                {
                    rot = Quaternion.LookRotation(req.Direction.Value);
                }

                var view = pool.Spawn(null, req.Position, rot);
                view.Play(req.Scale, req.Color);
                reqA.LifeTimes.Add(reqE) = new()
                {
                    Duration = view.Duration,
                    Time = view.Duration,
                };
                reqA.FXs.Add(reqE) = new()
                {
                    Pool = pool,
                    PooledInstance = view,
                };

                reqA.Events.TryAddOrGet(reqE);
            }

            reqA.Requests.ClearAll();
        }
    }
}
