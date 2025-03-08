using Asteroids.Components;
using Asteroids.Data;
using Asteroids.Utils;
using DCFApixels.DragonECS;

namespace Asteroids.Systems
{
    internal class RestartSystem : IEcsRun
    {
        [DI] private EcsWorld _world;
        [DI] private PoolService _poolService;

        private class Aspect : EcsAspect
        {
            public EcsTagPool<RestartEvent> RestartEvents = Inc;
        }

        private class PoolIdAspect : EcsAspect
        {
            public readonly EcsPool<PoolId> PoolIds = Inc;
        }
        
        public void Run()
        {
            foreach (var e in _world.Where(out Aspect a))
            {
                foreach (var poolE in _world.Where(out PoolIdAspect p))
                {
                    var poolId = p.PoolIds.Get(poolE);
                    _poolService.Return(poolId.Id, poolId.Component);
                }

                foreach (var entity in _world.Entities)
                {
                    _world.DelEntity(entity);
                }

                _world.GetPool<ChangeState>().Add(_world.NewEntity()).NewState = GameState.Play;
            }
        }
    }
}