using Asteroids.Components;
using Asteroids.Data;
using DCFApixels.DragonECS;
using JetBrains.Annotations;
using UnityEngine;

namespace Asteroids.Systems
{
    internal class InputSystem : IEcsRun, IEcsInit
    {
        [DI] private EcsDefaultWorld _world;
        [DI] private SceneData _sceneData;

        private class Aspect : EcsAspect
        {
            [UsedImplicitly]
            public readonly EcsPool<Starship> Starships = Inc;
            public readonly EcsTagPool<ShootEvent> ShootEvents = Exc;
        }

        private class InputDataAspect : EcsAspect
        {
            [UsedImplicitly]
            public readonly EcsPool<Starship> Starships = Inc;
            public readonly EcsPool<InputData> InputDatas = Inc;
        }

        public void Run()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                foreach (var e in _world.Where(out Aspect a))
                {
                    a.ShootEvents.Add(e);
                }
            }
            else
            {
                if (Input.touchSupported)
                {
                    var gameScreen = _sceneData.UI.GameScreen;
                    if (gameScreen.Shoot.IsDown)
                    {
                        gameScreen.Shoot.IsDown = false;
                        foreach (var e in _world.Where(out Aspect a))
                        {
                            a.ShootEvents.Add(e);
                        }
                    }
                }
            }


            var horizontal = Input.GetAxis("Horizontal");
            var vertical = Input.GetAxis("Vertical");
            
            foreach (var e in _world.Where(out InputDataAspect a))
            {
                ref var inputData = ref a.InputDatas.Get(e);
                inputData.Horizontal = horizontal;
                inputData.Vertical = vertical;
            }

            if (Input.touchSupported && horizontal == 0 && vertical == 0)
            {
                var gameScreen = _sceneData.UI.GameScreen;

                foreach (var e in _world.Where(out InputDataAspect a))
                {
                    ref var inputData = ref a.InputDatas.Get(e);

                    inputData.Horizontal = gameScreen.Left.IsDown ? -1f : gameScreen.Right.IsDown ? 1f : 0f;
                    inputData.Vertical = gameScreen.Acceleration.IsDown ? 1f : 0f;
                }
            }
        }

        public void Init()
        {
            Input.simulateMouseWithTouches = false;
            _sceneData.UI.GameScreen.MobileControlRoot.SetActive(Input.touchSupported);
        }
    }
}