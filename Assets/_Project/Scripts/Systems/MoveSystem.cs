using Asteroids.Components;
using Asteroids.Data;
using DCFApixels.DragonECS;
using UnityEngine;

namespace Asteroids.Systems
{
    internal class MoveSystem : IEcsRun
    {
        [DI] private EcsDefaultWorld _world;
        [DI] private RuntimeData _runtimeData;

        private class Aspect : EcsAspect
        {
            public readonly EcsPool<TransformRef> TransformRefs = Inc;
            public readonly EcsPool<MoveInfo> MoveInfos = Inc;
        }
    
        public void Run()
        {
            foreach (var e in _world.Where(out Aspect aspect))
            {
                var transform = aspect.TransformRefs.Get(e).Value;
                ref var moveInfo = ref aspect.MoveInfos.Get(e);

                moveInfo.Position += moveInfo.Speed * Time.deltaTime * moveInfo.Forward;
                if (moveInfo.Power != 0)
                {
                    moveInfo.Speed = Mathf.Clamp(moveInfo.Speed + moveInfo.Power * moveInfo.Acceleration * Time.deltaTime, -moveInfo.MaxSpeed, moveInfo.MaxSpeed);
                }
                else
                {
                    moveInfo.Speed *= 1 - moveInfo.Friction;
                }

                transform.position = moveInfo.Position;

                if (moveInfo.RotationSpeed != 0)
                {
                    transform.Rotate(Vector3.up, moveInfo.RotationSpeed * Time.deltaTime);
                    moveInfo.Forward = transform.forward;
                }
            }
        }
    }
}