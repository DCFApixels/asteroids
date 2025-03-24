using Asteroids.Components;
using Asteroids.Data;
using Asteroids.MovementFeature;
using Asteroids.Utils;
using DCFApixels.DragonECS;
using JetBrains.Annotations;

namespace Asteroids.Systems
{
    internal class KillOutsideSystem : IEcsRun
    {
        [DI] private EcsDefaultWorld _world;
        [DI] private RuntimeData _runtimeData;
        [DI] private StaticData _staticData;
        [DI] private PoolService _poolService;

        private class Aspect : EcsAspect
        {
            public readonly EcsPool<TransformData> TransformDatas = Inc;
            [UsedImplicitly]
            public readonly EcsTagPool<KillOutsideMarker> KillOutsideEvents = Inc;
            public readonly EcsPool<PoolId> PoolIds = Inc;
        }
    
        public void Run()
        {
            foreach (var e in _world.Where(out Aspect a))
            {
                var position = a.TransformDatas[e].position;
                var fieldSize = _runtimeData.FieldSize * _staticData.AdditionalKillOffset;
                if (position.x <= -fieldSize.x / 2f || position.x > fieldSize.x / 2f ||
                    position.z <= -fieldSize.y / 2f || position.z > fieldSize.y / 2f)
                {
                    var poolId = a.PoolIds.Get(e);
                    _poolService.Return(poolId.Id, poolId.Component);
                    _world.DelEntity(e);
                }
            }
        }
    }
}