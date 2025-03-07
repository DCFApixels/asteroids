using Asteroids.Components;
using Asteroids.Utils;
using DCFApixels.DragonECS;

namespace Asteroids.Systems
{
    internal class KillHitObjectSystem : IEcsRun
    {
        [DI] private EcsDefaultWorld _world;
        [DI] private PoolService _poolService;

        class Aspect : EcsAspect
        {
            public readonly EcsPool<TransformRef> TransformRefs = Inc;
            public readonly EcsPool<Hit> Hits = Inc;
            public readonly EcsPool<PoolID> PoolIDs = Inc;
        }

        public void Run()
        {
            foreach (var e in _world.Where(out Aspect a))
            {
                var poolId = a.PoolIDs.Get(e);
                _poolService.Return(poolId.Id, poolId.Component);
                _world.DelEntity(e);
            }
        }
    }
}