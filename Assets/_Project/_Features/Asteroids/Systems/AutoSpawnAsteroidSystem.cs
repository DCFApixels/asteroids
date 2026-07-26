using Asteroids.Components;
using DCFApixels.DragonECS;
using UnityEngine;

namespace Asteroids.AsteroidsFeature
{
    public class AutoSpawnAsteroidSystem : IEcsRun
    {
        [DI] EcsDefaultWorld _world;
        [DI] ConfigData c;
        [DI] RuntimeData r;

        int _previousSpawnTime;
        
        public void Run()
        {
            if (r.GameState != GameState.Play)
            {
                return;
            }

            var gameTime = (int)(Time.time - r.LevelStartTime);
            if (gameTime != _previousSpawnTime && gameTime >= c.SpawnFrequency &&
                gameTime % c.SpawnFrequency == 0)
            {
                _previousSpawnTime = gameTime;

                var spawnRequests = _world.GetPool<SpawnAsteroidRequest>();
                
                for (var i = 0; i < c.SpawnAmount; i++)
                {
                    var size = r.FieldSize;

                    var startAsteroidRadius = c.AsteroidDescription.BoundsRadius;

                    var spawnPosition = new Vector3(
                        Random.value > 0.5f
                            ? Random.Range(size.x / 2f + startAsteroidRadius / 2f, size.x / 2f + startAsteroidRadius/2f)
                            : Random.Range(-size.x / 2f - startAsteroidRadius, -size.x / 2f - startAsteroidRadius / 2f),
                        0,
                        Random.Range(-size.y / 2f - startAsteroidRadius/2f, size.y / 2f + startAsteroidRadius/2f));

                    var startRotation = Quaternion.LookRotation(-spawnPosition);
                    ref var spawnRequest = ref spawnRequests.Add(_world.NewEntity());
                    spawnRequest.Description = c.AsteroidDescription;
                    spawnRequest.Position = spawnPosition;
                    spawnRequest.Rotation = startRotation;
                    spawnRequest.OverrideDeathsCount = c.AsteroidDescription.DeathsCount;
                    spawnRequest.OverrideRadius = startAsteroidRadius;
                }
            }
        }
    }
}
