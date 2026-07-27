using Asteroids.LocalInputFeature;
using DCFApixels.DragonECS;
using Modules.Motion;
using UnityEngine;

namespace Asteroids.StarshipInputControlFeature
{
    [MetaGroup(StarshipInputControlModule.META_GROUP, EcsConsts.SYSTEMS_GROUP)]
    [MetaColor(StarshipInputControlModule.META_COLOR)]
    class StarshipMovementSystem : IEcsRun
    {
        [DI] EcsDefaultWorld _world;
        class Aspect : EcsAspect
        {
            public EcsPool<RigidTransform> RigidTransforms = Inc;
            public EcsPool<Velocity> Velocities = Inc;
            public EcsPool<StarshipMovementData> MovementDatas = Inc;
            public EcsPool<MoveAxisInputEvent> MoveAxisInputEvents = Inc;
        }
        public void Run()
        {
            foreach (var e in _world.Where(out Aspect a))
            {
                ref var rigidTransform = ref a.RigidTransforms[e];
                ref var velocity = ref a.Velocities[e];
                ref var movementData = ref a.MovementDatas[e];
                ref var moveAxisInput = ref a.MoveAxisInputEvents[e];

                var forward = rigidTransform.Rotation * Vector3.forward;
                forward.y = 0;
                forward = forward.normalized;

                var forwardProject = Vector3.Project(velocity.Lineral, forward);
                var forwardProjectSqrMag = forwardProject.sqrMagnitude;
                var maxSpeedSqr = movementData.MaxSpeed * movementData.MaxSpeed;

                if (forwardProjectSqrMag < maxSpeedSqr)
                {
                    var forwardAcceleration = forward * movementData.Acceleration * Time.deltaTime * Mathf.Clamp01(moveAxisInput.Vertical);
                    velocity.Lineral += forwardAcceleration;
                }

                var maxRotationSpeed = movementData.MaxRotationSpeed;

                if ((maxRotationSpeed > 0 && velocity.Angular.y < maxRotationSpeed) || velocity.Angular.y > maxRotationSpeed)
                {
                    velocity.Angular.y += maxRotationSpeed * moveAxisInput.Horizontal * Time.deltaTime;
                }
            }
        }
    }
}
