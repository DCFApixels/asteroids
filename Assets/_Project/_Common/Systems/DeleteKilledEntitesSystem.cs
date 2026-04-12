using Asteroids.Components;
using DCFApixels.DragonECS;

namespace Asteroids.Systems
{
    internal class DeleteKilledEntitesSystem : IEcsRun, IEcsDefaultAddParams
    {
        public AddParams AddParams => EcsConsts.POST_END_LAYER;

        [DI] EcsDefaultWorld _world;

        class Aspect : EcsAspect
        {
            public EcsPool<KillRequest> Requests = Inc;
        }
        public void Run()
        {
            foreach (var e in _world.Where(out Aspect a))
            {
                _world.DelEntity(e);
            }
            _world.ReleaseDelEntityBufferAll();
        }
    }
}