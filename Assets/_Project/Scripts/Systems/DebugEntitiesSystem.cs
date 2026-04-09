using Asteroids.MovementFeature;
using DCFApixels;
using DCFApixels.DragonECS;
using System.Linq;

namespace Asteroids.Systems
{
    class DebugEntitiesSystem : IEcsRun, IEcsDefaultAddParams
    {
        [DI] EcsDefaultWorld _world;

        public AddParams AddParams => EcsConsts.POST_END_LAYER;

        public void Run()
        {
            foreach (var e in _world.Where(out SingleAspect<RigidTransform> a))
            {
                DebugX.Draw().Text(a.pool[e].Position, e.ToString());
            }
        }
    }
}
