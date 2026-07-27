using Asteroids.Components;
using Asteroids.GameFieldFeature;
using DCFApixels.DragonECS;
using UnityEngine;

namespace Asteroids.AsteroidsFeature
{
    [MetaGroup(AsteroidsModule.META_GROUP, EcsConsts.SYSTEMS_GROUP)]
    [MetaColor(AsteroidsModule.META_COLOR)]
    class AutoSpawnAsteroidSystem : IEcsRun
    {
        [DI] EcsDefaultWorld _world;
        [DI] AsteroidsFeatureConfig _config;
        [DI] GameRuntimeData _gameRuntime;
        [DI] AsteroidsRuntimeData _runtime;
        [DI] GameFieldRuntimeData _gameFieldRuntime;

        int _previousSpawnTime;
        
        public void Run()
        {
            if (_gameRuntime.GameState != GameState.Play)
            {
                return;
            }

            var gameTime = (int)(Time.time - _runtime.LevelStartTime);
            if (gameTime != _previousSpawnTime && gameTime >= _config.SpawnFrequency &&
                gameTime % _config.SpawnFrequency == 0)
            {
                _previousSpawnTime = gameTime;

                var spawnRequests = _world.GetPool<SpawnAsteroidRequest>();
                
                for (var i = 0; i < _config.SpawnAmount; i++)
                {
                    var size = _gameFieldRuntime.FieldSize;

                    var startAsteroidRadius = _config.AsteroidDescription.BoundsRadius;

                    var spawnPosition = new Vector3(
                        Random.value > 0.5f
                            ? Random.Range(size.x / 2f + startAsteroidRadius / 2f, size.x / 2f + startAsteroidRadius/2f)
                            : Random.Range(-size.x / 2f - startAsteroidRadius, -size.x / 2f - startAsteroidRadius / 2f),
                        0,
                        Random.Range(-size.y / 2f - startAsteroidRadius/2f, size.y / 2f + startAsteroidRadius/2f));

                    var startRotation = Quaternion.LookRotation(-spawnPosition);
                    ref var spawnRequest = ref spawnRequests.Add(_world.NewEntity());
                    spawnRequest.Description = _config.AsteroidDescription;
                    spawnRequest.Position = spawnPosition;
                    spawnRequest.Rotation = startRotation;
                    spawnRequest.OverrideDeathsCount = _config.AsteroidDescription.DeathsCount;
                    spawnRequest.OverrideRadius = startAsteroidRadius;
                }
            }
        }
    }
}
