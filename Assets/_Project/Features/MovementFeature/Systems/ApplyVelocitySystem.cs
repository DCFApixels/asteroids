using DCFApixels.DragonECS;
using UnityEngine;

namespace Asteroids.MovementFeature
{
    [MetaGroup(MovementModule.META_GROUP)]
    [MetaColor(MovementModule.META_COLOR)]
    internal class ApplyVelocitySystem : IEcsRun, IEcsDefaultAddParams, IEcsFixedRun
    {
        public AddParams AddParams => EcsConsts.END_LAYER;

        [DI] EcsDefaultWorld _world;

        class VeloctityDragAspect : EcsAspect
        {
            public EcsPool<RigidbodyData> RigidbodyDatas = Inc;
            public EcsPool<Velocity> Velocities = Inc;
        }
        class TransformAspect : EcsAspect
        {
            public EcsPool<RigidTransform> RigidTransforms = Inc;
            public EcsPool<Velocity> Velocities = Inc;
        }
        public void FixedRun()
        {
            foreach (var e in _world.Where(out VeloctityDragAspect a))
            {
                ref var velocity = ref a.Velocities[e];
                ref var rigidbody = ref a.RigidbodyDatas[e];

                velocity.Lineral *= Mathf.Clamp01(1f - rigidbody.LineralDrag * Time.deltaTime);
                velocity.Angular *= Mathf.Clamp01(1f - rigidbody.AngularDrag * Time.deltaTime);
            }
        }

        public void Run()
        {
            foreach (var e in _world.Where(out TransformAspect a))
            {
                ref var transform = ref a.RigidTransforms[e];
                var velocity = a.Velocities.Read(e);

                transform.Position += velocity.Lineral * Time.deltaTime;
                Rotate(ref transform, velocity.Angular * Time.deltaTime);
            }
        }

        private void Rotate(ref RigidTransform transform, Vector3 velocity)
        {
            if(velocity.x == 0 && velocity.y == 0 && velocity.z == 0)
            {
                return;
            }
            Quaternion velocityRotation = Quaternion.Euler(velocity);
            transform.Rotation = transform.Rotation * ((Quaternion.Inverse(transform.Rotation) * velocityRotation) * transform.Rotation);
        }
    }
}
