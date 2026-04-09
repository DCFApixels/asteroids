using DCFApixels.DragonECS;
using UnityEngine;

namespace Asteroids.LocalInputFeature
{
    [MetaGroup(LocalInputModule.META_GROUP)]
    [MetaColor(LocalInputModule.META_COLOR)]
    internal class LocalInputSystem : IEcsRun, IEcsInit
    {
        [DI] EcsDefaultWorld _world;
        [DI] SceneData s;

        class InputAspect : EcsAspect
        {
            public EcsPool<LocalInputReceiver> LocalInputReceivers = Inc;
            public EcsPool<MoveAxisInputEvent> MoveAxisInputEvents = Opt;
            public EcsPool<FireInputBeginEvent> FireInputBeginEvents = Opt;
        }
        public void Run()
        {
            var horizontal = Input.GetAxis("Horizontal");
            var vertical = Input.GetAxis("Vertical");
            if (Input.touchSupported && horizontal == 0 && vertical == 0)
            {
                var gameScreen = s.UI.GameScreen;
                horizontal = gameScreen.Left.IsDown ? -1f : gameScreen.Right.IsDown ? 1f : 0f;
                vertical = gameScreen.Acceleration.IsDown ? 1f : 0f;
            }
            bool isSpaceDown = Input.GetKeyDown(KeyCode.Space);
            if (Input.touchSupported && isSpaceDown == false)
            {
                var gameScreen = s.UI.GameScreen;
                isSpaceDown = gameScreen.Shoot.IsDown;
            }

            _world.GetAspects(out InputAspect a);
            a.FireInputBeginEvents.ClearAll();

            bool hasMoveInput = horizontal != 0 || vertical != 0;
            foreach (var e in _world.Where(a))
            {
                if (isSpaceDown)
                {
                    a.FireInputBeginEvents.TryAddOrGet(e);
                }
                else 
                { 
                    a.FireInputBeginEvents.TryDel(e);
                }

                if (hasMoveInput)
                {
                    ref var moveEvent = ref a.MoveAxisInputEvents.TryAddOrGet(e);
                    moveEvent.Horizontal = horizontal;
                    moveEvent.Vertical = vertical;
                }
                else if (a.MoveAxisInputEvents.Has(e))
                {
                    ref var moveEvent = ref a.MoveAxisInputEvents[e];
                    if (moveEvent.Axis != Vector2.zero)
                    {
                        moveEvent.Axis = Vector2.zero;
                    }
                    else
                    {
                        a.MoveAxisInputEvents.Del(e);
                    }
                }
            }
        }

        public void Init()
        {
            Input.simulateMouseWithTouches = false;
            s.UI.GameScreen.MobileControlRoot.SetActive(Input.touchSupported);
        }
    }
}