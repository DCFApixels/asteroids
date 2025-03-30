using Asteroids.BoundsOverlapsFeature;
using Asteroids.Components;
using Asteroids.Data;
using DCFApixels.DragonECS;
using System.Collections.Generic;

namespace Asteroids.StartshipsFeature
{
    internal class RespawnStarShipOnHitSystem : IEcsRun, IEcsInit
    {
        [DI] EcsDefaultWorld _world;
        [DI] RuntimeData _runtimeData;
        [DI] SceneData _sceneData;

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

            if (starshipA.Starships.Count != 0 || _runtimeData.GameState != GameState.Play)
            {
                return;
            }
            
            _runtimeData.LifeLeft--;
            if (_runtimeData.LifeLeft == 0)
            {
                _world.GetPool<ChangeState>().Add(_world.NewEntity()).NextState = GameState.Lose;
            }
            else
            {
                //kill all asteroids near spawn point. Fully kill!
                _runtimeData.AreaGrid.FindAllInRadius(_sceneData.SpawnPosition.position.x,
                    _sceneData.SpawnPosition.position.z, _sceneData.KillOnSpawnRadius, _hits);
                foreach (var hit in _hits)
                {
                    if (hit.Id.TryGetID(out var asteroidEntity))
                    {
                        //_asteroids.TryAddOrGet(asteroidEntity).DeathsLeft = 0;
                        //_hitEvents.TryAddOrGet(asteroidEntity);
                    }
                }


                _world.GetPool<SpawnStarshipEvent>().NewEntity();
            }
        }

        public void Init()
        {
            //_hitEvents = _world.GetPool<OverlapsEvent>();
            _asteroids = _world.GetPool<Asteroid>();
        }
    }
}