using Asteroids.Components;
using Asteroids.Data;
using Asteroids.Utils;
using DCFApixels.DragonECS;
using UnityEngine;

namespace Asteroids.Systems
{
    internal class SpawnAsteroidSystem : IEcsRun
    {
        [DI] private StaticData _staticData;
        [DI] private EcsDefaultWorld _world;
        [DI] private PoolService _poolService;

        class EventAspect : EcsAspect
        {
            public readonly EcsPool<SpawnAsteroidEvent> SpawnAsteroids = Inc;
        }
        class AsteroidAspect : EcsAspect
        {
            public readonly EcsPool<TransformRef> TransformRefs = Inc;
            public readonly EcsTagPool<DeleteOutside> DeleteOutsides = Inc;
            public readonly EcsPool<Asteroid> Asteroids = Inc;
            public readonly EcsPool<PoolID> PoolIDs = Inc;
            public readonly EcsPool<MoveInfo> MoveInfos = Inc;
        }

        public void Run()
        {
            var asteroidA = _world.GetAspect<AsteroidAspect>();
            foreach (var eventE in _world.Where(out EventAspect eventA))
            {
                ref var spawnAsteroid = ref eventA.SpawnAsteroids.Get(eventE);

                var prefab = _staticData.AsteroidView;
                var instance = _poolService.Get(prefab, out var id);
                instance.transform.position = spawnAsteroid.Position;
                instance.transform.rotation = spawnAsteroid.Rotation;

                var asteroidE = _world.NewEntity(asteroidA);
                ref var asteroid = ref asteroidA.Asteroids.Get(asteroidE);
                ref var poolID = ref asteroidA.PoolIDs.Get(asteroidE);
                ref var transformRef = ref asteroidA.TransformRefs.Get(asteroidE);
                ref var moveInfo = ref asteroidA.MoveInfos.Get(asteroidE);

                asteroid.View = instance;
                asteroid.DeathsLeft = spawnAsteroid.DeathsLeft;
                asteroid.Radius = spawnAsteroid.StartRadius;
                asteroid.View.transform.localScale = Vector3.one * spawnAsteroid.StartRadius / instance.Radius;

                poolID.Id = id;
                poolID.Component = instance;

                transformRef.Value = instance.transform;

                moveInfo.Speed = Random.Range(_staticData.AsteroidMinSpeed, _staticData.AsteroidMaxSpeed);
                moveInfo.Position = instance.transform.position;
                moveInfo.Forward = instance.transform.forward;

                eventA.SpawnAsteroids.Del(eventE);
            }
        }

    }
}