﻿using Asteroids.ControlsFeature;
using Asteroids.MovementFeature;
using DCFApixels.DragonECS;
using UnityEngine;

namespace Asteroids.StarshipMovmentFeature
{
    [MetaGroup(StarshipMovmentModule.META_GROUP)]
    [MetaColor(StarshipMovmentModule.META_COLOR)]
    public class StarshipMovmentSystem : IEcsRun
    {
        class Aspect : EcsAspect
        {
            public EcsPool<TransformData> transformDatas = Inc;
            public EcsPool<Velocity> velocities = Inc;
            public EcsPool<StarshipMovmentData> movementDatas = Inc;
            public EcsPool<AxisControlData> axisControlDatas = Inc;
        }
        [DI] EcsDefaultWorld _world;
        public void Run()
        {
            foreach (var e in _world.Where(out Aspect a))
            {
                ref var transformData = ref a.transformDatas.Get(e);
                ref var velocity = ref a.velocities.Get(e);
                ref var movementData = ref a.movementDatas.Get(e);
                ref var axisControlData = ref a.axisControlDatas.Get(e);

                var forward = transformData.rotation * Vector3.forward;
                forward.y = 0;
                forward = forward.normalized;

                var forwardProject = Vector3.Project(velocity.lineral, forward);
                var forwardProjectSqrMag = forwardProject.sqrMagnitude;
                var maxSpeedSqr = movementData.MaxSpeed * movementData.MaxSpeed;

                if (forwardProjectSqrMag < maxSpeedSqr)
                {
                    var forwardAcceleration = forward * movementData.Acceleration * Time.deltaTime * Mathf.Clamp01(axisControlData.Axis.y);
                    velocity.lineral += forwardAcceleration;
                }

                float rotangle = movementData.MaxRotationSpeed;

                if (rotangle > 0 && velocity.angular.y < rotangle || velocity.angular.y > rotangle)
                {
                    velocity.angular.y += rotangle * axisControlData.Axis.x * Time.deltaTime;
                }
            }
        }
    }
}
