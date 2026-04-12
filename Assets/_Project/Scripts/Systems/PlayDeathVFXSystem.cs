using Asteroids.Components;
using Asteroids.Views;
using DCFApixels.DragonECS;
using Modules.FX;

namespace Asteroids
{
    internal class PlayDeathVFXSystem : IEcsRun
    {
        [DI] EcsDefaultWorld _world;
        class Aspect : EcsAspect
        {
            public EcsRefPool<ViewBase> Views = Inc;
            public EcsPool<KillRequest> KillRequests = Inc;
            public EcsPool<ShortVFXSpawnRequest> ShortVFXSpawnRequest = Opt;
        }
        public void Run()
        {
            foreach (var e in _world.Where(out Aspect a))
            {
                var view = a.Views[e];
                if (view.DeathVFX)
                {
                    ref var r = ref a.ShortVFXSpawnRequest.NewEntity();
                    r.Prefab = view.DeathVFX;
                    r.Position = view.transform.position;
                    r.Scale = view.GetScale();
                }
            }
        }
    }
}
