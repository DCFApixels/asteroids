using Asteroids.Components;
using Asteroids.Data;
using Asteroids.MovementFeature;
using Asteroids.Utils;
using DCFApixels.DragonECS;
using UnityEngine;

namespace Asteroids.Systems
{
    internal class CheckAsteroidHitSystem : IEcsRun
    {
        [DI] RuntimeData _runtimeData;
        [DI] EcsDefaultWorld _world;
        [DI] PoolService _poolService;
        [DI] StaticData _staticData;

        private class Aspect : EcsAspect
        {
            public EcsPool<Asteroid> Asteroids = Inc;
            public EcsPool<HitSignal> HitEvents = Inc;
            public EcsPool<TransformData> TransformDatas = Inc;
        }
        public void Run()
        {
            foreach (var e in _world.Where(out Aspect a))
            {
                ref var asteroid = ref a.Asteroids[e];
                ref var asteroidTransformData = ref a.TransformDatas[e];
                
                var explosion = _poolService.Get(_staticData.AsteroidExplosionPrefab, out var instanceID);
                explosion.transform.position = asteroidTransformData.position;
                explosion.Play(_poolService, instanceID);
                
                _runtimeData.Score++;

                if (asteroid.DeathsLeft <= 0)
                {
                    continue;
                }
                
                asteroid.DeathsLeft--;
                
                var forward = Vector3.forward;
                
                var hitByObjectEntLong = a.HitEvents[e].ByObject;
                if (hitByObjectEntLong.TryUnpack(out var hitByEntity, out short _))
                {
                    var hitByTransformData = a.TransformDatas[hitByEntity];
                    if (hitByTransformData.position != asteroidTransformData.position)
                    {
                        forward = asteroidTransformData.position - hitByTransformData.position;
                    }
                }

                var spawnPool = _world.GetPool<SpawnAsteroidEvent>();
                
                for (var i = 0; i < 2; i++)
                {
                    var startForward = Quaternion.Euler(0, 90 + 180 * i, 0) * forward;

                    ref var spawnAsteroid = ref spawnPool.NewEntity();
                    spawnAsteroid.DeathsLeft = asteroid.DeathsLeft;
                    spawnAsteroid.Position = asteroidTransformData.position;
                    spawnAsteroid.Rotation = Quaternion.LookRotation(startForward);
                    spawnAsteroid.StartRadius = asteroid.Radius / 2f;
                }
            }
        }
    }
}