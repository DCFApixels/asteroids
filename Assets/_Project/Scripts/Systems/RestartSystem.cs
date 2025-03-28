using Asteroids.Components;
using Asteroids.Data;
using Asteroids.Utils;
using DCFApixels.DragonECS;
using JetBrains.Annotations;

namespace Asteroids.Systems
{
    internal class RestartSystem : IEcsRun
    {
        [DI] private EcsWorld _world;
        [DI] private PoolService _poolService;

        private class Aspect : EcsAspect
        {
            [UsedImplicitly]
            public readonly EcsTagPool<RestartEvent> RestartEvents = Inc;
        }

        private class PoolIdAspect : EcsAspect
        {
            public readonly EcsPool<PooledUnit> PoolIds = Inc;
        }
        
        public void Run()
        {
            foreach (var _ in _world.Where(out Aspect _))
            {
                foreach (var poolE in _world.Where(out PoolIdAspect p))
                {
                    var poolId = p.PoolIds.Get(poolE);
                    _poolService.Return(poolId.ID, poolId.Unit);
                }

                foreach (var entity in _world.Entities)
                {
                    _world.DelEntity(entity);
                }

                _world.GetPool<ChangeState>().Add(_world.NewEntity()).NextState = GameState.Play;
            }
        }
    }
}