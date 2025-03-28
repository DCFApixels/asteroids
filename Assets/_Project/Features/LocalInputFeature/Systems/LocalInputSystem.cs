using Asteroids.Data;
using DCFApixels.DragonECS;
using UnityEngine;

namespace Asteroids.LocalInputFeature
{
    internal class LocalInputSystem : IEcsRun, IEcsInit
    {
        [DI] EcsDefaultWorld _world;
        [DI] SceneData _sceneData;

        class InputAspect : EcsAspect
        {
            public EcsPool<LocalInputReceiver> LocalInputReceivers = Inc;
            public EcsPool<MoveAxisInputSignal> MoveAxisInputSignals = Opt;
            public EcsPool<FireInputBeginSignal> FireInputBeginSignals = Opt;
        }
        public void Run()
        {
            var horizontal = Input.GetAxis("Horizontal");
            var vertical = Input.GetAxis("Vertical");
            if (Input.touchSupported && horizontal == 0 && vertical == 0)
            {
                var gameScreen = _sceneData.UI.GameScreen;
                horizontal = gameScreen.Left.IsDown ? -1f : gameScreen.Right.IsDown ? 1f : 0f;
                vertical = gameScreen.Acceleration.IsDown ? 1f : 0f;
            }
            bool isSpaceDown = Input.GetKeyDown(KeyCode.Space);
            if (Input.touchSupported && isSpaceDown == false)
            {
                var gameScreen = _sceneData.UI.GameScreen;
                isSpaceDown = gameScreen.Shoot.IsDown;
            }


            bool hasMoveInput = horizontal != 0 || vertical != 0;
            foreach (var e in _world.Where(out InputAspect a))
            {
                if (isSpaceDown)
                {
                    a.FireInputBeginSignals.TryAddOrGet(e);
                }
                else 
                { 
                    a.FireInputBeginSignals.TryDel(e);
                }

                if (hasMoveInput)
                {
                    ref var inputData = ref a.MoveAxisInputSignals.TryAddOrGet(e);
                    inputData.Horizontal = horizontal;
                    inputData.Vertical = vertical;
                }
                else if (a.MoveAxisInputSignals.Has(e))
                {
                    ref var inputData = ref a.MoveAxisInputSignals[e];
                    if (inputData.Axis != Vector2.zero)
                    {
                        inputData.Axis = Vector2.zero;
                    }
                    else
                    {
                        a.MoveAxisInputSignals.Del(e);
                    }
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