using Asteroids.Components;
using DCFApixels.DragonECS;

namespace Asteroids.Systems
{
    class DeleteKilledEntitiesSystem : IEcsRun, IEcsDefaultAddParams
    {
        public AddParams AddParams => EcsConsts.POST_END_LAYER;

        [DI] EcsDefaultWorld _world;

        class Aspect : EcsAspect
        {
            public EcsPool<KillRequest> KillRequests = Inc;
        }
        public void Run()
        {
            foreach (var e in _world.Where(out Aspect _))
            {
                _world.DelEntity(e);
            }
            _world.ReleaseDelEntityBufferAll();
        }
    }
}
