using Asteroids.Components;
using DCFApixels.DragonECS;

namespace Asteroids.Systems
{
    internal class SetMoveInfoFromInputDataSystem : IEcsRun
    {
        [DI] private EcsDefaultWorld _world;
        class InputDataAspect : EcsAspect
        {
            public readonly EcsPool<InputData> InputDatas = Inc;
            public readonly EcsPool<MoveInfo> MoveInfos = Inc;
        }

        public void Run()
        {
            foreach (var e in _world.Where(out InputDataAspect a))
            {
                ref var inputData = ref a.InputDatas.Get(e);
                ref var moveInfo = ref a.MoveInfos.Get(e);

                moveInfo.Speed = inputData.Vertical * moveInfo.DefaultSpeed;
                moveInfo.RotationSpeed = inputData.Horizontal * moveInfo.DefaultRotationSpeed;
            }
        }
    }
}