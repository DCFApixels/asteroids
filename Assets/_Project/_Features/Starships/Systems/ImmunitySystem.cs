using DCFApixels.DragonECS;
using UnityEngine;

namespace Asteroids.StarshipsFeature
{
    internal class ImmunitySystem : IEcsRun
    {
        [DI] private EcsDefaultWorld _world;
        [DI] private EcsGraphWorld _graphWorld;

        private class Aspect : EcsAspect
        {
            public readonly EcsPool<HitImmunity> Immunities = Inc;
            public readonly EcsPool<Starship> Starships = Opt;
        }
        public void Run()
        {
            foreach (var e in _graphWorld.Where(out Aspect a))
            {
                ref var immunity = ref a.Immunities[e];
                immunity.TimeLeft -= Time.deltaTime;
                if (immunity.TimeLeft <= 0)
                {
                    a.Immunities.Del(e);
                }
            }
            foreach (var e in _world.Where(out Aspect a))
            {
                ref var immunity = ref a.Immunities[e];
                immunity.TimeLeft -= Time.deltaTime;


                if (a.Starships.Has(e))
                {
                    var starshipView = a.Starships[e].View;

                    if (immunity.TimeLeft <= 0)
                    {
                        starshipView.BlinkFromValueReset();
                    }
                    else
                    {
                        starshipView.BlinkFromValue(immunity.TimeLeft);
                    }
                }


                if (immunity.TimeLeft <= 0)
                {
                    a.Immunities.Del(e);
                }
            }
        }
    }
}