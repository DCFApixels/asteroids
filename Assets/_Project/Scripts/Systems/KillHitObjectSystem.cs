using Asteroids.Components;
using Asteroids.Utils;
using DCFApixels.DragonECS;

namespace Asteroids.Systems
{
    internal class KillHitObjectSystem : IEcsRun, IEcsDefaultAddParams
    {
        public AddParams AddParams => EcsConsts.POST_END_LAYER;

        [DI] private EcsDefaultWorld _world;
        [DI] private PoolService _poolService;

        class Aspect : EcsAspect
        {
            public EcsPool<PooledUnit> PoolIds = Inc;
            public EcsPool<KillSignal> KillSignals = Inc;
        }

        public void Run()
        {
            foreach (var e in _world.Where(out Aspect a))
            {
                var poolId = a.PoolIds.Get(e);
                _poolService.Return(poolId.ID, poolId.Unit);
                _world.DelEntity(e);
            }
        }
    }
}