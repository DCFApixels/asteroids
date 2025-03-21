using DCFApixels.DragonECS;
using System.Linq;

namespace Asteroids.MovementFeature
{
    internal class DebugVelocitySystem : IEcsRun
    {
        [DI] EcsDefaultWorld _world;
        class VeloctityViewAspect : EcsAspect
        {
            public EcsPool<TransformData> transformDatas = Inc;
            public EcsPool<Velocity> velocities = Inc;
        }
        public void Run()
        {
            foreach (var e in _world.Where(out VeloctityViewAspect a))
            {
                ref var velocity = ref a.velocities.Get(e);
                ref var transform = ref a.transformDatas.Get(e);

                //DebugX.Draw.Ray(transform.position, velocity.lineral / 2f, Color.cyan);
            }
        }
    }
}
