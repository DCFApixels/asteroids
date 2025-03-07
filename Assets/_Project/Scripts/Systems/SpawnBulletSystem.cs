using Asteroids.Components;
using Asteroids.Data;
using Asteroids.Utils;
using DCFApixels.DragonECS;
using System;

namespace Asteroids.Systems
{
    internal class SpawnBulletSystem : IEcsRun
    {
        [DI] private EcsDefaultWorld _world;
        [DI] private StaticData _staticData;
        [DI] private PoolService _poolService;

        class StarshipAspect : EcsAspect
        {
            public readonly EcsPool<Starship> Starships = Inc;
            public readonly EcsPool<MoveInfo> MoveInfoss = Inc;
            public readonly EcsTagPool<WantShootSignal> WantShootSignals = Inc;
        }
        class BulletAspect : EcsAspect
        {
            public readonly EcsPool<TransformRef> TransformRefs = Inc;
            public readonly EcsPool<Bullet> Bullets = Inc;
            public readonly EcsPool<MoveInfo> MoveInfos = Inc;
            public readonly EcsPool<Cycle> Cycles = Inc;
            public readonly EcsPool<PoolID> poolIDs = Inc;
            public readonly EcsPool<RequestIntersection> RequestIntersections = Inc;
        }


        public void Run()
        {
            var bulletA = _world.GetAspect<BulletAspect>();
            foreach (var starshipE in _world.Where(out StarshipAspect starshipA))
            {
                var starshipView = starshipA.Starships.Get(starshipE).View;
                ref var starshipMoveInfo = ref starshipA.MoveInfoss.Get(starshipE);

                var preafb = _staticData.BulletView;
                var instance = _poolService.Get(preafb, out var id);
                instance.transform.position = starshipView.transform.position;
                instance.transform.rotation = starshipView.transform.rotation;

                var bulletE = _world.NewEntity(bulletA);
                ref var bullet = ref bulletA.Bullets.Get(bulletE);
                ref var transformRef = ref bulletA.TransformRefs.Get(bulletE);
                ref var moveInfo = ref bulletA.MoveInfos.Get(bulletE);
                ref var poolId = ref bulletA.poolIDs.Get(bulletE);
                ref var cycle = ref bulletA.Cycles.Get(bulletE);
                ref var requestIntersection = ref bulletA.RequestIntersections.Get(bulletE);

                bullet.View = instance;

                transformRef.Value = instance.transform;

                moveInfo.Speed = _staticData.BulletSpeed + Math.Abs(starshipMoveInfo.Speed);
                moveInfo.Position = instance.transform.position;
                moveInfo.Forward = instance.transform.forward;

                poolId.Id = id;
                poolId.Component = instance;

                requestIntersection.CheckRadius = _staticData.AsteroidView.Radius + instance.Radius;
                requestIntersection.ObjectRadius = instance.Radius;

                starshipA.WantShootSignals.Del(starshipE);
            }
        }
    }
}