using Asteroids.Components;
using Asteroids.Data;
using Asteroids.LocalInputFeature;
using Asteroids.MovementFeature;
using Asteroids.Utils;
using DCFApixels.DragonECS;
using System;
using UnityEngine;

namespace Asteroids.StartshipsFeature
{
    internal class SpawnBulletSystem : IEcsRun
    {
        [DI] private EcsDefaultWorld _world;
        [DI] private StaticData _staticData;
        [DI] private PoolService _poolService;

        class StashipAspect : EcsAspect
        {
            public readonly EcsPool<Starship> Starships = Inc;
            public readonly EcsPool<FireInputBeginSignal> FireInputBeginSignals = Inc;
            public readonly EcsPool<TransformData> TransformDatas = Inc;
            public readonly EcsPool<Velocity> Velocities = Inc;
        }
        class SpawnAspect : EcsAspect
        {
            public readonly EcsPool<PooledUnit> PoolIDs = Inc;
            public readonly EcsPool<Velocity> Velocities = Inc;
            public readonly EcsPool<TransformData> TransformDatas = Inc;
            public readonly EcsPool<RequestIntersectionEvent> RequestIntersectionEvents = Inc;
        }

        public void Run()
        {
            var spawnA = _world.GetAspect<SpawnAspect>();
            foreach (var stashipE in _world.Where(out StashipAspect stashipA))
            {
                var stashipTransformData = stashipA.TransformDatas.Get(stashipE);

                var newE = _world.NewEntity(_staticData.BulletTemplate); 
                var newViewInstance = _poolService.Get(_staticData.BulletViewPrefab, out spawnA.PoolIDs.TryAddOrGet(newE));
                newViewInstance.Connect((_world, newE), false);
                spawnA.Apply(_world, newE);

                ref var newTransformData = ref spawnA.TransformDatas[newE];
                newTransformData.position = stashipTransformData.position;
                newTransformData.rotation = stashipTransformData.rotation;

                ref var newVelocity = ref spawnA.Velocities[newE];
                newVelocity.lineral = newTransformData.CalcLocalVector(Vector3.forward) * (_staticData.BulletSpeed + Math.Abs(stashipA.Velocities[stashipE].lineral.magnitude));

                ref var newRequestIntersectionEvent = ref spawnA.RequestIntersectionEvents[newE];
                newRequestIntersectionEvent.CheckRadius = _staticData.AsteroidViewPrefab.Radius + newViewInstance.Radius;
                newRequestIntersectionEvent.ObjectRadius = newViewInstance.Radius;

                stashipA.FireInputBeginSignals.Del(stashipE);
            }
        }
    }
}