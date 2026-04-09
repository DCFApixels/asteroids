using Asteroids;
using Asteroids.MovementFeature;
using DCFApixels.DragonECS;

namespace Modules.BoundsOverlaps
{
    [MetaGroup(BoundsOverlapsModule.META_GROUP)]
    [MetaColor(BoundsOverlapsModule.META_COLOR)]
    internal class RecalculateSpaceHashSystem : IEcsRun, IEcsDefaultAddParams
    {
        public AddParams AddParams => EcsConsts.PRE_BEGIN_LAYER;

        [DI] EcsDefaultWorld _world;
        [DI] BoundsOverlapsRuntime r;

        public void Run()
        {
            r.AreaGrid.Clear();

            foreach (var e in _world.Where(out SingleAspect<RigidTransform>  a))
            {
                var position = a.pool[e].Position;
                r.AreaGrid.Add((e, _world), position.x, position.z);
            }
        }
    }
}