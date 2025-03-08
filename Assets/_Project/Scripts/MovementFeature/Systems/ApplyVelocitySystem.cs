using DCFApixels.DragonECS;
using UnityEngine;

namespace Asteroids.MovementFeature
{
    [MetaGroup(nameof(MovementModule), EcsConsts.SYSTEMS_GROUP)]
    [MetaColor(MetaColor.Cyan)]
    internal class ApplyVelocitySystem : IEcsRun, IEcsDefaultAddParams, IEcsFixedRunProcess
    {
        public AddParams AddParams => EcsConsts.END_LAYER;


        class VeloctityDragAspect : EcsAspect
        {
            public EcsPool<Velocity> velocities = Inc;
            public EcsPool<RigidbodyData> rigidbodyDatas = Inc;
        }
        class TransformAspect : EcsAspect
        {
            public EcsPool<TransformData> transformDatas = Inc;
            public EcsPool<Velocity> velocities = Inc;
        }

        [DI] EcsDefaultWorld _world;

        public void FixedRun()
        {
            foreach (var e in _world.Where(out VeloctityDragAspect a))
            {
                ref var velocity = ref a.velocities.Get(e);
                ref var rigidbody = ref a.rigidbodyDatas.Get(e);

                velocity.lineral = velocity.lineral * Mathf.Clamp01(1f - rigidbody.lineralDrag * Time.deltaTime);
            }
        }

        public void Run()
        {
            foreach (var e in _world.Where(out TransformAspect a))
            {
                ref var transform = ref a.transformDatas.Get(e);
                var velocity = a.velocities.Read(e);

                transform.position += velocity.lineral * Time.deltaTime;
                Rotate(ref transform, velocity.angular * Time.deltaTime);
            }
        }

        private void Rotate(ref TransformData transform, Vector3 velocity)
        {
            if(velocity.x == 0 && velocity.y == 0 && velocity.z == 0)
            {
                return;
            }
            Quaternion velocityRotation = Quaternion.Euler(velocity);
            transform.rotation = transform.rotation * ((Quaternion.Inverse(transform.rotation) * velocityRotation) * transform.rotation);
        }
    }
}
