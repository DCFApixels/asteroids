using DCFApixels;
using DCFApixels.DragonECS;
using System.Linq;
using UnityEngine;

namespace Asteroids.MovementFeature
{
    [MetaGroup(MovementModule.META_GROUP)]
    [MetaColor(MovementModule.META_COLOR)]
    internal class DebugVelocitySystem : IEcsRun, IEcsDefaultAddParams
    {
        public AddParams AddParams => EcsConsts.POST_END_LAYER;

        [DI] EcsDefaultWorld _world;

        class VeloctityViewAspect : EcsAspect
        {
            public EcsPool<RigidTransform> RigidTransform = Inc;
            public EcsPool<Velocity> Velocities = Inc;
        }
        public void Run()
        {
            foreach (var e in _world.Where(out VeloctityViewAspect a))
            {
                ref var velocity = ref a.Velocities[e];
                ref var transform = ref a.RigidTransform[e];

                DebugX.Draw(new Color(1, 1, 1, 0.3f) * Color.cyan).RayArrow(transform.Position, velocity.Lineral / 2f);
            }
        }
    }
}