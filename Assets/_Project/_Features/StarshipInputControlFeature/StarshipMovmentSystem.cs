using Asteroids.LocalInputFeature;
using DCFApixels.DragonECS;
using Modules.Motion;
using UnityEngine;

namespace Asteroids.StarshipInputControlFeature
{
    [MetaGroup(StarshipInputControlModule.META_GROUP)]
    [MetaColor(StarshipInputControlModule.META_COLOR)]
    public class StarshipMovmentSystem : IEcsRun
    {
        [DI] EcsDefaultWorld _world;
        class Aspect : EcsAspect
        {
            public EcsPool<RigidTransform> RigidTransforms = Inc;
            public EcsPool<Velocity> Velocities = Inc;
            public EcsPool<StarshipMovmentData> MovementDatas = Inc;
            public EcsPool<MoveAxisInputEvent> MoveAxisInputSignals = Inc;
        }
        public void Run()
        {
            _world.GetAspects(out Aspect a);

            foreach (var e in _world.Where(a))
            {
                ref var rigidTransform = ref a.RigidTransforms[e];
                ref var velocity = ref a.Velocities[e];
                ref var movementData = ref a.MovementDatas[e];
                ref var moveAxisInputSignal = ref a.MoveAxisInputSignals[e];

                var forward = rigidTransform.Rotation * Vector3.forward;
                forward.y = 0;
                forward = forward.normalized;

                var forwardProject = Vector3.Project(velocity.Lineral, forward);
                var forwardProjectSqrMag = forwardProject.sqrMagnitude;
                var maxSpeedSqr = movementData.MaxSpeed * movementData.MaxSpeed;

                if (forwardProjectSqrMag < maxSpeedSqr)
                {
                    var forwardAcceleration = forward * movementData.Acceleration * Time.deltaTime * Mathf.Clamp01(moveAxisInputSignal.Vertical);
                    velocity.Lineral += forwardAcceleration;
                }

                float rotangle = movementData.MaxRotationSpeed;

                if (rotangle > 0 && velocity.Angular.y < rotangle || velocity.Angular.y > rotangle)
                {
                    velocity.Angular.y += rotangle * moveAxisInputSignal.Horizontal * Time.deltaTime;
                }
            }
        }
    }
}
