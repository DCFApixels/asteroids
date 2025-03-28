using Asteroids.LocalInputFeature;
using Asteroids.MovementFeature;
using DCFApixels.DragonECS;
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
            public EcsPool<TransformData> TransformDatas = Inc;
            public EcsPool<Velocity> Velocities = Inc;
            public EcsPool<StarshipMovmentData> MovementDatas = Inc;
            public EcsPool<MoveAxisInputSignal> MoveAxisInputSignals = Inc;
        }
        public void Run()
        {
            foreach (var e in _world.Where(out Aspect a))
            {
                ref var transformData = ref a.TransformDatas.Get(e);
                ref var velocity = ref a.Velocities.Get(e);
                ref var movementData = ref a.MovementDatas.Get(e);
                ref var moveAxisInputSignal = ref a.MoveAxisInputSignals.Get(e);

                var forward = transformData.rotation * Vector3.forward;
                forward.y = 0;
                forward = forward.normalized;

                var forwardProject = Vector3.Project(velocity.lineral, forward);
                var forwardProjectSqrMag = forwardProject.sqrMagnitude;
                var maxSpeedSqr = movementData.MaxSpeed * movementData.MaxSpeed;

                if (forwardProjectSqrMag < maxSpeedSqr)
                {
                    var forwardAcceleration = forward * movementData.Acceleration * Time.deltaTime * Mathf.Clamp01(moveAxisInputSignal.Vertical);
                    velocity.lineral += forwardAcceleration;
                }

                float rotangle = movementData.MaxRotationSpeed;

                if (rotangle > 0 && velocity.angular.y < rotangle || velocity.angular.y > rotangle)
                {
                    velocity.angular.y += rotangle * moveAxisInputSignal.Horizontal * Time.deltaTime;
                }
            }
        }
    }
}
