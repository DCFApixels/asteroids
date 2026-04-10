using DCFApixels.DragonECS;

namespace Modules.FX
{
    [MetaGroup(FXModule.META_GROUP)]
    [MetaColor(FXModule.META_COLOR)]
    public class SpawnFXSystem : IEcsRun
    {
        [DI] EcsDefaultWorld _world;
        class RequestAspect : EcsAspect
        {
            public EcsPool<SpawnFXRequest> Requests = Inc;
            public EcsPool<FXSpawnedEvent> Events = Opt;
        }
        public void Run()
        {
            _world.GetAspects(out RequestAspect reqA);
            foreach (var reqE in _world.Where(reqA))
            {
                ref var req = ref reqA.Requests[reqE];

                req.Template.Apply(_world.ID, reqE);
                reqA.Events.TryAddOrGet(reqE);
            }

            reqA.Requests.ClearAll();
        }
    }
}
