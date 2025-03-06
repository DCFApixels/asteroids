using Asteroids.Components;
using DCFApixels.DragonECS;
using UnityEngine;

namespace Asteroids.Systems
{
    internal class InputSystem : IEcsRun
    {
        [DI] private EcsDefaultWorld _world;

        class Aspect : EcsAspect
        {
            public EcsPool<Starship> Starship = Inc;
            public readonly EcsTagPool<WantShoot> WantShoot = Exc;
        }

        class InputDataAspect : EcsAspect
        {
            public EcsPool<Starship> Starship = Inc;
            public readonly EcsPool<InputData> Input = Inc;
        }
    
        public void Run()
        {
            if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
            {
                foreach (var e in _world.Where(out Aspect a))
                {
                    a.WantShoot.Add(e);
                }
            }

            foreach (var e in _world.Where(out InputDataAspect a))
            {
                ref var inputData = ref a.Input.Get(e);
                inputData.Horizontal = Input.GetAxis("Horizontal");
                inputData.Vertical = Input.GetAxis("Vertical");
            }
        }
    }
}