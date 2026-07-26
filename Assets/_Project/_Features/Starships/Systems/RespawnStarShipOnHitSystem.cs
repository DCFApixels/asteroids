using Asteroids.AsteroidsFeature;
using Asteroids.Components;
using DCFApixels.DragonECS;
using Modules.BoundsOverlaps;
using System.Collections.Generic;

namespace Asteroids.StarshipsFeature
{
    internal class RespawnStarShipOnHitSystem : IEcsRun, IEcsInit
    {
        [DI] EcsDefaultWorld _world;
        [DI] RuntimeData r;
        [DI] SceneData s;

        //private EcsPool<OverlapsEvent> _hitEvents;
        private EcsPool<Asteroid> _asteroids;

        class StarshipAspect : EcsAspect
        {
            public EcsPool<Starship> Starships = Inc;
        }

        private readonly List<AreaGrid2D<entlong>.Hit> _hits = new(32);

        public void Run()
        {
            var starshipA = _world.GetAspect<StarshipAspect>();

            if (starshipA.Starships.Count != 0 || r.GameState != GameState.Play)
            {
                return;
            }

            r.LifeLeft--;
            if (r.LifeLeft == 0)
            {
                _world.GetPool<ChangeState>().Add(_world.NewEntity()).NextState = GameState.Lose;
            }
            else
            {
                //kill all asteroids near spawn point. Fully kill!
                r.BoundsOverlapsRuntime.AreaGrid.FindAllInRadius(s.SpawnPlayerPosition.position.x,
                    s.SpawnPlayerPosition.position.z, s.KillOnSpawnRadius, _hits);
                foreach (var hit in _hits)
                {
                    if (hit.Id.TryGetID(out var asteroidEntity))
                    {
                        //_asteroids.TryAddOrGet(asteroidEntity).DeathsLeft = 0;
                        //_hitEvents.TryAddOrGet(asteroidEntity);
                    }
                }


                _world.GetPool<SpawnStarshipRequest>().NewEntity();
            }
        }

        public void Init()
        {
            //_hitEvents = _world.GetPool<OverlapsEvent>();
            _asteroids = _world.GetPool<Asteroid>();
        }
    }
}