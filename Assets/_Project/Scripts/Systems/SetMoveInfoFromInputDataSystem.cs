using Asteroids.Components;
using DCFApixels.DragonECS;

namespace Asteroids.Systems
{
    internal class SetMoveInfoFromInputDataSystem : IEcsRun
    {
        [DI] private EcsDefaultWorld _world;

        private class InputDataAspect : EcsAspect
        {
            public readonly EcsPool<InputData> InputDatas = Inc;
            public readonly EcsPool<MoveInfo> MoveInfos = Inc;
        }
    
        public void Run()
        {
            foreach (var e in _world.Where(out InputDataAspect a))
            {
                var inputData = a.InputDatas.Get(e);
                ref var move = ref a.MoveInfos.Get(e);

                move.Power = inputData.Vertical;
                move.RotationSpeed = inputData.Horizontal * move.DefaultRotationSpeed;
            }
        }
    }
}