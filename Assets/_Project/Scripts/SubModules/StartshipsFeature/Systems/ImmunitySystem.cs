using Asteroids.Components;
using DCFApixels.DragonECS;
using UnityEngine;

namespace Asteroids.StartshipsFeature
{
    internal class ImmunitySystem : IEcsRun
    {
        [DI] private EcsWorld _world;

        private class Aspect : EcsAspect
        {
            public readonly EcsPool<Starship> Starships = Inc;
            public readonly EcsPool<Immunity> Immunities = Inc;
        }
        public void Run()
        {
            foreach (var e in _world.Where(out Aspect a))
            {
                ref var immunity = ref a.Immunities.Get(e);
                immunity.TimeLeft -= Time.deltaTime;
                var starshipView = a.Starships.Get(e).View;
                
                if (immunity.TimeLeft <= 0)
                {
                    a.Immunities.Del(e);
                    starshipView.BlinkFromValueReset();
                }
                else
                {
                    starshipView.BlinkFromValue(immunity.TimeLeft);
                }
            }
        }
    }
}