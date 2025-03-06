using System;
using Asteroids.Components;
using Asteroids.Data;
using Asteroids.Utils;
using DCFApixels.DragonECS;

namespace Asteroids.Systems
{
    internal class SpawnBulletSystem : IEcsRun
    {
        [DI] private EcsDefaultWorld _world;
        [DI] private StaticData _staticData;
        [DI] private PoolService _poolService;

        class Aspect : EcsAspect
        {
            public readonly EcsPool<Starship> Starship = Inc;
            public readonly EcsTagPool<WantShoot> WantShoot = Inc;
            public readonly EcsPool<MoveInfo> MoveInfos = Inc;
        }
    
        public void Run()
        {
            foreach (var e in _world.Where(out Aspect a))
            {
                var starshipView = a.Starship.Get(e).View;
            
                var instance = _poolService.Get(_staticData.BulletView, out var id);
                instance.transform.position = starshipView.transform.position;
                instance.transform.rotation = starshipView.transform.rotation;

                var entity = _world.NewEntity();
                _world.GetPool<Bullet>().Add(entity).View = instance;
                _world.GetPool<TransformRef>().Add(entity).Value = instance.transform;
                ref var moveInfo = ref _world.GetPool<MoveInfo>().Add(entity);
                moveInfo.Speed = _staticData.BulletSpeed + Math.Abs(a.MoveInfos.Get(e).Speed);
                moveInfo.Position = instance.transform.position;
                moveInfo.Forward = instance.transform.forward;
            
                ref var poolId = ref _world.GetPool<PoolId>().Add(entity);
                poolId.Id = id;
                poolId.Component = instance;

                _world.GetPool<Cycle>().Add(entity);
                ref var asteroid = ref _world.GetPool<RequestIntersection>().Add(entity);
                asteroid.CheckRadius = _staticData.AsteroidView.Radius + instance.Radius;
                asteroid.ObjectRadius = instance.Radius;

                a.WantShoot.Del(e);
            }
        }
    }
}