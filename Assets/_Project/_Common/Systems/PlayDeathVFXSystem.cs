using Asteroids.Components;
using Asteroids.Views;
using DCFApixels.DragonECS;
using Modules.BoundsOverlaps;
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

            public EcsPool<ShortVFXSpawnRequest> ShortVFXSpawnRequests = Opt;
        }
        public void Run()
        {
            foreach (var e in _world.Where(out Aspect a))
            {
                ref var rec = ref a.KillRequests[e];
                var view = a.Views[e];
                if (view.DeathVFX)
                {
    
                    ref var request = ref a.ShortVFXSpawnRequests.NewEntity();
                    request.Prefab = view.DeathVFX;
                    request.Position = view.transform.position;
                    request.Scale = view.GetScale();
                    if (rec.Normal != null)
                    {
                        request.Direction = rec.Normal.Value;
                    }
                }
            }
        }
    }
}
