using Asteroids.Components;
using DCFApixels.DragonECS;
using UnityEngine;

namespace Asteroids.Systems
{
    internal class InputSystem : IEcsRun
    {
        [DI] EcsDefaultWorld _world;

        class Aspect : EcsAspect
        {
            public readonly EcsPool<Starship> Starships = Inc;
            public readonly EcsTagPool<WantShootSignal> WantShoots = Exc;
        }
        class InputDataAspect : EcsAspect
        {
            public readonly EcsPool<Starship> Starships = Inc;
            public readonly EcsPool<InputData> InputDatas = Inc;
        }

        public void Run()
        {
            if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
            {
                foreach (var e in _world.Where(out Aspect a))
                {
                    a.WantShoots.Add(e);
                }
            }

            foreach (var e in _world.Where(out InputDataAspect a))
            {
                ref var inputData = ref a.InputDatas.Get(e);
                inputData.Horizontal = Input.GetAxis("Horizontal");
                inputData.Vertical = Input.GetAxis("Vertical");
            }
        }
    }
}