using Asteroids.Components;
using Asteroids.Data;
using DCFApixels.DragonECS;
using UnityEngine;

namespace Asteroids.Systems
{
    public class AutoSpawnAsteroidSystem : IEcsRun
    {
        [DI] private StaticData _staticData;
        [DI] private EcsDefaultWorld _world;
        [DI] private RuntimeData _runtimeData;

        private int _previousSpawnTime = 0;
        public void Run()
        {
            var second = (int)Time.time;
            if (second != _previousSpawnTime && second >= _staticData.SpawnFrequency &&
                second % _staticData.SpawnFrequency == 0)
            {
                _previousSpawnTime = second;

                for (int var = 0; var < _staticData.SpawnAmount; var++)
                {
                    var size = _runtimeData.FieldSize;

                    var startAsteroidRadius = _staticData.AsteroidView.Radius;
               
                    var spawnPosition = new Vector3(
                        Random.value > 0.5f
                            ? Random.Range(size.x / 2f + startAsteroidRadius / 2f, size.x / 2f + startAsteroidRadius/2f)
                            : Random.Range(-size.x / 2f - startAsteroidRadius, -size.x / 2f - startAsteroidRadius / 2f),
                        0,
                        Random.Range(-size.y / 2f - startAsteroidRadius/2f, size.y / 2f + startAsteroidRadius/2f));

                    var startRotation = Quaternion.LookRotation(-spawnPosition);
                    ref var spawnAsteroid = ref _world.GetPool<SpawnAsteroid>().Add(_world.NewEntity());
                    spawnAsteroid.Position = spawnPosition;
                    spawnAsteroid.Rotation = startRotation;
                    spawnAsteroid.DeathsLeft = _staticData.AsteroidDeathLeft;
                    spawnAsteroid.StartRadius = startAsteroidRadius;
                }
            }
        }
    }
}