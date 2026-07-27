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

                var rotationSpeed = CalculateRotationSpeed(movementData, velocity.Lineral);

                velocity.Angular.y += rotationSpeed * moveAxisInput.Horizontal * Time.deltaTime;
                velocity.Angular.y = Mathf.Clamp(velocity.Angular.y, -rotationSpeed, rotationSpeed);
            }
        }

        float CalculateRotationSpeed(StarshipMovementData movementData, Vector3 velocity)
        {
            var minRotationSpeed = Mathf.Max(0f, movementData.MinRotationSpeed);
            var maxRotationSpeed = Mathf.Max(minRotationSpeed, movementData.MaxRotationSpeed);

            var velocityOnGamePlane = new Vector2(velocity.x, velocity.z).magnitude;
            var threshold = movementData.MaxRotationSpeedVelocityThreshold > 0f
                ? movementData.MaxRotationSpeedVelocityThreshold
                : movementData.MaxSpeed;
            var speedT = threshold > 0f ? Mathf.Clamp01(velocityOnGamePlane / threshold) : 1f;

            return Mathf.Lerp(minRotationSpeed, maxRotationSpeed, speedT);
        }
    }
}
