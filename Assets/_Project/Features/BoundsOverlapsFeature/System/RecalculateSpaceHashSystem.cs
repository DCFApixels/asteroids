using Asteroids.Data;
using Asteroids.MovementFeature;
using DCFApixels.DragonECS;

namespace Asteroids.BoundsOverlapsFeature
{
    [MetaGroup(BoundsOverlapsModule.META_GROUP)]
    [MetaColor(BoundsOverlapsModule.META_COLOR)]
    internal class RecalculateSpaceHashSystem : IEcsRun, IEcsDefaultAddParams
    {
        public AddParams AddParams => EcsConsts.PRE_BEGIN_LAYER;

        [DI] private RuntimeData _runtimeData;
        [DI] private EcsDefaultWorld _world;

        public void Run()
        {
            _runtimeData.AreaGrid.Clear();

            //foreach (var e in _world.Where(out  CombinedAspect<SingleAspect<TransformData>, SingleAspect<Asteroid>>  a))
            //{
            //    var position = a.a0.pool[e].position;
            //    _runtimeData.AreaHash.Add((e, _world), position.x, position.z);
            //}

            foreach (var e in _world.Where(out SingleAspect<TransformData>  a))
            {
                var position = a.pool[e].position;
                _runtimeData.AreaGrid.Add((e, _world), position.x, position.z);
            }

        }
    }
}