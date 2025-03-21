using System.Collections.Generic;
using Asteroids.Components;
using Asteroids.Data;
using Asteroids.Utils;
using DCFApixels.DragonECS;

namespace Asteroids.Systems
{
    internal class RespawnStarShipOnHitSystem : IEcsRun, IEcsInit
    {
        [DI] private EcsDefaultWorld _world;
        [DI] private RuntimeData _runtimeData;
        [DI] private SceneData _sceneData;

        private EcsPool<Starship> _starships;
        private EcsPool<HitEvent> _hitEvents;
        private EcsPool<Asteroid> _asteroids;
        
        private readonly List<AreaHash2D<entlong>.Hit> _hits = new(32);

        public void Run()
        {
            if (_starships.Count != 0 || _runtimeData.GameState != GameState.Play)
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
                    var hitId = hit.Id;

                    if (hitId.TryUnpack(out var asteroidEntity, out short _))
                    {
                        _asteroids.TryAddOrGet(asteroidEntity).DeathsLeft = 0;
                        _hitEvents.TryAddOrGet(asteroidEntity);
                    }
                }

                _world.GetPool<SpawnStarshipEvent>().Add(_world.NewEntity());
            }
        }

        public void Init()
        {
            _starships = _world.GetPool<Starship>();
            _hitEvents = _world.GetPool<HitEvent>();
            _asteroids = _world.GetPool<Asteroid>();
        }
    }
}