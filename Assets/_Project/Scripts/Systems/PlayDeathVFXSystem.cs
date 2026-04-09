using Asteroids.Components;
using Asteroids.Views;
using DCFApixels.DragonECS;
using UnityEngine;

namespace Asteroids
{
    internal class PlayDeathVFXSystem : IEcsRun
    {
        [DI] EcsDefaultWorld _world;
        class Aspect : EcsAspect
        {
            public EcsRefPool<ViewBase> Views = Inc;
            public EcsPool<KillRequest> KillRequests = Inc;
        }
        public void Run()
        {
            foreach (var e in _world.Where(out Aspect a))
            {
                var view = a.Views[e];
                if (view.DeathVFX)
                {
                    var vfx = view.DeathVFX.Spawn(_world, view.transform.position, Quaternion.identity);
                    vfx.view.Play();
                }
            }
        }
    }
}
