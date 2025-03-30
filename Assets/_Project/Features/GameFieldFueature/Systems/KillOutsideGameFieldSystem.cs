using Asteroids.Components;
using Asteroids.Data;
using Asteroids.MovementFeature;
using Asteroids.Utils;
using DCFApixels.DragonECS;

namespace Asteroids.GameFieldFueature
{
    [MetaGroup(GameFieldModule.META_GROUP)]
    [MetaColor(GameFieldModule.META_COLOR)]
    internal class KillOutsideGameFieldSystem : IEcsRun
    {
        [DI] EcsDefaultWorld _world;
        [DI] RuntimeData _runtimeData;
        [DI] StaticData _staticData;
        [DI] PoolService _poolService;

        private class Aspect : EcsAspect
        {
            public EcsPool<TransformData> TransformDatas = Inc;
            public EcsTagPool<KillOutsideGameFieldMarker> KillOutsideEvents = Inc;
            public EcsPool<PooledUnit> PoolIds = Inc;
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
                    _poolService.Return(poolId.ID, poolId.Unit);
                    _world.DelEntity(e);
                }
            }
        }
    }
}