﻿using Asteroids.Components;
using Asteroids.Utils;
using DCFApixels.DragonECS;
using JetBrains.Annotations;

namespace Asteroids.Systems
{
    internal class KillHitObjectSystem : IEcsRun, IEcsDefaultAddParams
    {
        [DI] private EcsDefaultWorld _world;
        [DI] private PoolService _poolService;

        public AddParams AddParams => EcsConsts.POST_END_LAYER;

        private class AspectPool : EcsAspect
        {
            [UsedImplicitly]
            public readonly EcsPool<HitSignal> HitEvents = Inc;
            public readonly EcsPool<PoolID> PoolIds = Inc;
        }
    
        public void Run()
        {
            foreach (var e in _world.Where(out AspectPool a))
            {
                var poolId = a.PoolIds.Get(e);
                _poolService.Return(poolId.Id, poolId.Component);
                _world.DelEntity(e);
            }
        }
    }
}