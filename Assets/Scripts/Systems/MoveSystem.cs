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

        class Aspect : EcsAspect
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
                moveInfo.Speed += moveInfo.Acceleration * Time.deltaTime;

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