using Asteroids.Components;
using Asteroids.Data;
using Asteroids.MovementFeature;
using Asteroids.Utils;
using DCFApixels.DragonECS;
using UnityEngine;

namespace Asteroids.Systems
{
    internal class SpawnAsteroidSystem : IEcsRun
    {
        [DI] StaticData _staticData;
        [DI] EcsDefaultWorld _world;
        [DI] PoolService _poolService;

        private class EventAspect : EcsAspect
        {
            public EcsPool<SpawnAsteroidEvent> SpawnAsteroidEvents = Inc;
        }
        class SpawnAspect : EcsAspect
        {
            public EcsPool<PooledUnit> PoolIDs = Inc;
            public EcsPool<Asteroid> Asteroids = Inc;
            public EcsPool<Velocity> Velocities = Inc;
            public EcsPool<TransformData> TransformDatas = Inc;
        }

        public void Run()
        {
            var spawnA = _world.GetAspect<SpawnAspect>();
            foreach (var eventE in _world.Where(out EventAspect eventA))
            {
                var spawnAsteroidEvent = eventA.SpawnAsteroidEvents.Get(eventE);

                var newE = _world.NewEntity(_staticData.AsteroidTemplate);
                var newViewInstance = _poolService.Get(_staticData.AsteroidViewPrefab, out spawnA.PoolIDs.TryAddOrGet(newE));
                newViewInstance.Connect((_world, newE), false);
                spawnA.Apply(_world, newE);

                newViewInstance.SetRadius(spawnAsteroidEvent.StartRadius);

                ref var newAsteroid = ref spawnA.Asteroids.TryAddOrGet(newE);
                newAsteroid.DeathsLeft = spawnAsteroidEvent.DeathsLeft;
                newAsteroid.Radius = spawnAsteroidEvent.StartRadius;

            
                ref var newTransformData = ref spawnA.TransformDatas.TryAddOrGet(newE);
                newTransformData.position = spawnAsteroidEvent.Position;
                newTransformData.rotation = spawnAsteroidEvent.Rotation;

                ref var newVelocity = ref spawnA.Velocities.TryAddOrGet(newE);
                newVelocity.lineral = newTransformData.CalcLocalVector(Vector3.forward) * Random.Range(_staticData.AsteroidMinSpeed, _staticData.AsteroidMaxSpeed);
            
                eventA.SpawnAsteroidEvents.Del(eventE);
            }
        }

    }
}