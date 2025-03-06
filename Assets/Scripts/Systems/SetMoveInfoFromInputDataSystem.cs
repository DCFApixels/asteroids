using Asteroids.Components;
using DCFApixels.DragonECS;

namespace Asteroids.Systems
{
    internal class SetMoveInfoFromInputDataSystem : IEcsRun
    {
        [DI] private EcsDefaultWorld _world;
        class InputDataAspect : EcsAspect
        {
            public readonly EcsPool<InputData> Input = Inc;
            public readonly EcsPool<MoveInfo> Move = Inc;
        }
    
        public void Run()
        {
            foreach (var e in _world.Where(out InputDataAspect a))
            {
                var inputData = a.Input.Get(e);
                ref var move = ref a.Move.Get(e);

                move.Speed = inputData.Vertical * move.DefaultSpeed;
                move.RotationSpeed = inputData.Horizontal * move.DefaultRotationSpeed;
            }
        }
    }
}