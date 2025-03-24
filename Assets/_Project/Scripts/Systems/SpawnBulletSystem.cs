using Asteroids.Components;
using Asteroids.Data;
using Asteroids.MovementFeature;
using Asteroids.Utils;
using DCFApixels.DragonECS;
using System;
using UnityEngine;

namespace Asteroids.Systems
{
    internal class SpawnBulletSystem : IEcsRun
    {
        [DI] private EcsDefaultWorld _world;
        [DI] private StaticData _staticData;
        [DI] private PoolService _poolService;

        private class StashipAspect : EcsAspect
        {
            public readonly EcsPool<Starship> Starships = Inc;
            public readonly EcsTagPool<ShootEvent> ShootEvents = Inc;
            public readonly EcsPool<TransformData> TransformDatas = Inc;
            public readonly EcsPool<Velocity> Velocities = Inc;
        }
        private class BulletAspect : EcsAspect
        {
            public readonly EcsPool<Starship> Starships = Inc;
            public readonly EcsTagPool<ShootEvent> ShootEvents = Inc;
            public readonly EcsPool<TransformData> TransformDatas = Inc;
            public readonly EcsPool<Velocity> Velocities = Inc;
        }

        public void Run()
        {
            var bulletA = _world.GetAspect<BulletAspect>();
            foreach (var stashipE in _world.Where(out StashipAspect stashipA))
            {
                var stashipTransformData = stashipA.TransformDatas.Get(stashipE);
            
                var bulletViewInstance = _poolService.Get(_staticData.BulletView, out var bulletViewInstanceID);

                var bulletE = _world.NewEntity(_staticData.BulletTemplate);
                _world.GetPool<Bullet>().Add(bulletE);
                bulletViewInstance.Connect((_world, bulletE), false);
                //_world.GetPool<UnityComponent<Transform>>().Add(bulletE).obj = bulletViewInstance.transform;
                ref var bulletTransformData = ref _world.GetPool<TransformData>().TryAddOrGet(bulletE);
                ref var bulletVelocity = ref _world.GetPool<Velocity>().TryAddOrGet(bulletE);
                bulletTransformData.position = stashipTransformData.position;
                bulletTransformData.rotation = stashipTransformData.rotation;
                bulletVelocity.lineral = stashipTransformData.rotation * Vector3.forward * (_staticData.BulletSpeed + Math.Abs(stashipA.Velocities[stashipE].lineral.magnitude));

                ref var poolId = ref _world.GetPool<PoolId>().TryAddOrGet(bulletE);
                poolId.Id = bulletViewInstanceID;
                poolId.Component = bulletViewInstance;

                _world.GetPool<WrapAroundScreenMarker>().TryAddOrGet(bulletE);
                _world.GetPool<KillOutsideMarker>().Add(bulletE);
                
                ref var asteroid = ref _world.GetPool<RequestIntersectionEvent>().TryAddOrGet(bulletE);
                asteroid.CheckRadius = _staticData.AsteroidView.Radius + bulletViewInstance.Radius;
                asteroid.ObjectRadius = bulletViewInstance.Radius;

                stashipA.ShootEvents.Del(stashipE);
            }
        }
    }
}