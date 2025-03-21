using System;
using Asteroids.Components;
using Asteroids.Data;
using Asteroids.Utils;
using DCFApixels.DragonECS;
using JetBrains.Annotations;

namespace Asteroids.Systems
{
    internal class SpawnBulletSystem : IEcsRun
    {
        [DI] private EcsDefaultWorld _world;
        [DI] private StaticData _staticData;
        [DI] private PoolService _poolService;

        private class Aspect : EcsAspect
        {
            [UsedImplicitly]
            public readonly EcsPool<Starship> Starships = Inc;
            public readonly EcsPool<TransformRef> TransformRefs = Inc;
            public readonly EcsTagPool<ShootEvent> ShootEvents = Inc;
            public readonly EcsPool<MoveInfo> MoveInfos = Inc;
        }
    
        public void Run()
        {
            foreach (var e in _world.Where(out Aspect a))
            {
                var transform = a.TransformRefs.Get(e).Value;
            
                var instance = _poolService.Get(_staticData.BulletView, out var id);
                instance.transform.position = transform.position;
                instance.transform.rotation = transform.rotation;

                var entity = _world.NewEntity();
                _world.GetPool<Bullet>().Add(entity);
                _world.GetPool<TransformRef>().Add(entity).Value = instance.transform;
                ref var moveInfo = ref _world.GetPool<MoveInfo>().Add(entity);
                moveInfo.Speed = moveInfo.MaxSpeed = _staticData.BulletSpeed + Math.Abs(a.MoveInfos.Get(e).Speed);
                moveInfo.Position = instance.transform.position;
                moveInfo.Forward = instance.transform.forward;
                moveInfo.Power = 1;
            
                ref var poolId = ref _world.GetPool<PoolId>().Add(entity);
                poolId.Id = id;
                poolId.Component = instance;

                _world.GetPool<WrapAroundScreenMarker>().Add(entity);
                _world.GetPool<KillOutsideMarker>().Add(entity);
                
                ref var asteroid = ref _world.GetPool<RequestIntersectionEvent>().Add(entity);
                asteroid.CheckRadius = _staticData.AsteroidView.Radius + instance.Radius;
                asteroid.ObjectRadius = instance.Radius;

                a.ShootEvents.Del(e);
            }
        }
    }
}