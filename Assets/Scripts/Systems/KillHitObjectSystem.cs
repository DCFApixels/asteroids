using Asteroids.Components;
using Asteroids.Utils;
using DCFApixels.DragonECS;

namespace Asteroids.Systems
{
    internal class KillHitObjectSystem : IEcsRun
    {
        [DI] private EcsDefaultWorld _world;
        [DI] private PoolService _poolService;
    
        class AspectPool : EcsAspect
        {
            public EcsPool<TransformRef> Transform = Inc;
            public EcsPool<Hit> Hit = Inc;
            public readonly EcsPool<PoolId> Pool = Inc;
        }
    
        public void Run()
        {
            foreach (var e in _world.Where(out AspectPool a))
            {
                var poolId = a.Pool.Get(e);
                _poolService.Return(poolId.Id, poolId.Component);
                _world.DelEntity(e);
            }
        }
    }
}