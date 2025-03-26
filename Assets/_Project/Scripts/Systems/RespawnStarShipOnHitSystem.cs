using Asteroids.Components;
using Asteroids.Data;
using Asteroids.MovementFeature;
using Asteroids.Utils;
using DCFApixels.DragonECS;
using System.Collections.Generic;

namespace Asteroids.Systems
{
    internal class RespawnStarShipOnHitSystem : IEcsRun, IEcsInit
    {
        [DI] EcsDefaultWorld _world;
        [DI] RuntimeData _runtimeData;
        [DI] SceneData _sceneData;

        private EcsPool<HitSignal> _hitEvents;
        private EcsPool<Asteroid> _asteroids;


        class StarshipAspect : EcsAspect
        {
            public EcsPool<Starship> Starships = Inc;
        }

        private readonly List<AreaHash2D<entlong>.Hit> _hits = new(32);

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
                _world.GetPool<ChangeState>().Add(_world.NewEntity()).NewState = GameState.Lose;
            }
            else
            {
                //kill all asteroids near spawn point. Fully kill!
                _runtimeData.AreaHash.FindAllInRadius(_sceneData.SpawnPosition.position.x,
                    _sceneData.SpawnPosition.position.z, _sceneData.KillOnSpawnRadius, _hits);
                foreach (var hit in _hits)
                {
                    if (hit.Id.TryGetID(out var asteroidEntity))
                    {
                        _asteroids.TryAddOrGet(asteroidEntity).DeathsLeft = 0;
                        _hitEvents.TryAddOrGet(asteroidEntity);
                    }
                }


                _world.GetPool<SpawnStarshipEvent>().NewEntity();
            }
        }

        public void Init()
        {
            _hitEvents = _world.GetPool<HitSignal>();
            _asteroids = _world.GetPool<Asteroid>();
        }
    }
}