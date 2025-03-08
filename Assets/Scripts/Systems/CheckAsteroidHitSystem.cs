using Asteroids.Components;
using Asteroids.Data;
using Asteroids.Utils;
using DCFApixels.DragonECS;
using UnityEngine;

namespace Asteroids.Systems
{
    internal class CheckAsteroidHitSystem : IEcsRun
    {
        [DI] private RuntimeData _runtimeData;
        [DI] private EcsDefaultWorld _world;
        [DI] private PoolService _poolService;
        [DI] private StaticData _staticData;

        private class Aspect : EcsAspect
        {
            public readonly EcsPool<Asteroid> Asteroids = Inc;
            public readonly EcsPool<HitEvent> HitEvents = Inc;
            public readonly EcsPool<MoveInfo> MoveInfos = Inc;
        }
        public void Run()
        {
            foreach (var e in _world.Where(out Aspect a))
            {
                ref var asteroid = ref a.Asteroids.Get(e);

                if (asteroid.DeathsLeft <= 0)
                {
                    continue;
                }
                
                asteroid.DeathsLeft--;
                _runtimeData.Score++;

                ref var asteroidPosition = ref a.MoveInfos.Get(e).Position;

                var explosion = _poolService.Get(_staticData.AsteroidExplosion, out var instanceID);
                explosion.transform.position = asteroidPosition;
                explosion.Play(_poolService, instanceID);

                var forward = Vector3.forward;
                
                var hitByObjectEntLong = a.HitEvents.Get(e).ByObject;
                if (hitByObjectEntLong.TryUnpack(out var hitByEntity, out short _))
                {
                    var hitByPosition = a.MoveInfos.Get(hitByEntity).Position;
                    if (hitByPosition != asteroidPosition)
                    {
                        forward = asteroidPosition - hitByPosition;
                    }
                }

                var spawnPool = _world.GetPool<SpawnAsteroidEvent>();
                
                for (var i = 0; i < 2; i++)
                {
                    var startForward = Quaternion.Euler(0, 90 + 180 * i, 0) * forward;

                    ref var spawnAsteroid = ref spawnPool.Add(_world.NewEntity());
                    spawnAsteroid.DeathsLeft = asteroid.DeathsLeft;
                    spawnAsteroid.Position = asteroidPosition;
                    spawnAsteroid.Rotation = Quaternion.LookRotation(startForward);
                    spawnAsteroid.StartRadius = asteroid.Radius / 2f;
                }
            }
        }
    }
}