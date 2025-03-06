using Asteroids.Components;
using Asteroids.Data;
using Asteroids.Utils;
using DCFApixels.DragonECS;

namespace Asteroids.Systems
{
    internal class KillOutsideSystem : IEcsRun
    {
        [DI] private EcsDefaultWorld _world;
        [DI] private RuntimeData _runtimeData;
        [DI] private StaticData _staticData;
        [DI] private PoolService _poolService;

        class Aspect : EcsAspect
        {
            public readonly EcsPool<TransformRef> Transform = Inc;
            public EcsTagPool<DeleteOutside> DeleteOutsize = Inc;
            public readonly EcsPool<PoolId> Pool = Inc;
        }
    
        public void Run()
        {
            foreach (var e in _world.Where(out Aspect a))
            {
                var transform = a.Transform.Get(e).Value;
                var position = transform.position;
                var fieldSize = _runtimeData.FieldSize * _staticData.AdditionalKillOffset;
                if (position.x <= -fieldSize.x / 2f || position.x > fieldSize.x / 2f ||
                    position.z <= -fieldSize.y / 2f || position.z > fieldSize.y / 2f)
                {
                    var poolId = a.Pool.Get(e);
                    _poolService.Return(poolId.Id, poolId.Component);
                    _world.DelEntity(e);
                }
            }
        }
    }
}