using Asteroids.ControlsFeature;
using Asteroids.MovementFeature;
using DCFApixels.DragonECS;
using UnityEngine;

namespace Asteroids.ShipMovementFeature
{
    [MetaGroup(ShipMovementModule.META_GROUP)]
    [MetaColor(ShipMovementModule.META_COLOR)]
    public class ShipMovmentSystem : IEcsRun
    {
        class Aspect : EcsAspect
        {
            public EcsPool<TransformData> transformDatas = Inc;
            public EcsPool<Velocity> velocities = Inc;
            public EcsPool<ShipMovementData> movementDatas = Inc;
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

                velocity.angular.y = movementData.MaxRotationSpeed * axisControlData.Axis.x;
            }
        }
    }
}
