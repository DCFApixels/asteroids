using DCFApixels.DragonECS;
using DCFApixels;
using System.Linq;
using UnityEngine;

namespace Asteroids.MovementFeature
{
    [MetaGroup(MovementModule.META_GROUP)]
    [MetaColor(MovementModule.META_COLOR)]
    internal class DebugVelocitySystem : IEcsRun
    {
        class VeloctityViewAspect : EcsAspect
        {
            public EcsPool<TransformData> transformDatas = Inc;
            public EcsPool<Velocity> velocities = Inc;
        }
        [DI] EcsDefaultWorld _world;
        public void Run()
        {
            foreach (var e in _world.Where(out VeloctityViewAspect a))
            {
                ref var velocity = ref a.velocities.Get(e);
                ref var transform = ref a.transformDatas.Get(e);

                DebugX.Draw(Color.cyan).RayArrow(transform.position, velocity.lineral / 2f);
            }
        }
    }
}
