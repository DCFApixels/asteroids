using DCFApixels.DragonECS;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Asteroids.LocalInputFeature
{
    [MetaGroup(LocalInputModule.META_GROUP)]
    [MetaColor(LocalInputModule.META_COLOR)]
    internal class LocalInputSystem : IEcsRun, IEcsInit
    {
        [DI] EcsDefaultWorld _world;
        [DI] SceneData s;
        [DI] ConfigData c;

        class InputAspect : EcsAspect
        {
            public EcsPool<LocalInputReceiver> LocalInputReceivers = Inc;
            public EcsPool<MoveAxisInputEvent> MoveAxisInputEvents = Opt;
            public EcsPool<FireInputBeginEvent> FireInputBeginEvents = Opt;
        }
        public void Run()
        {
            Vector2 moveAxis = ReadMoveAxis();
            bool isFirePressed = ReadFirePressedThisFrame();

            _world.GetAspects(out InputAspect a);
            a.FireInputBeginEvents.ClearAll();

            bool hasMoveInput = moveAxis != Vector2.zero;
            foreach (var e in _world.Where(a))
            {
                if (isFirePressed)
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
                    moveEvent.Axis = moveAxis;
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
            c.MoveAction?.action?.Enable();
            c.FireAction?.action?.Enable();
            s.UI.GameScreen.MobileControlRoot.SetActive(c.ShowMobileControlsOnTouchDevices && IsTouchSupported());
        }

        private Vector2 ReadMoveAxis()
        {
            Vector2 axis = c.MoveAction != null && c.MoveAction.action != null
                ? c.MoveAction.action.ReadValue<Vector2>()
                : ReadMoveAxisFromDevices();
            return axis != Vector2.zero ? axis : ReadMoveAxisFromMobileUi();
        }

        private bool ReadFirePressedThisFrame()
        {
            bool isPressed = c.FireAction != null && c.FireAction.action != null
                ? c.FireAction.action.WasPressedThisFrame()
                : ReadFirePressedFromDevices();
            return isPressed || ReadFirePressedFromMobileUi();
        }

        private Vector2 ReadMoveAxisFromMobileUi()
        {
            if (IsTouchSupported() == false)
            {
                return Vector2.zero;
            }

            var gameScreen = s.UI.GameScreen;
            float horizontal = gameScreen.Left.IsDown ? -1f : gameScreen.Right.IsDown ? 1f : 0f;
            float vertical = gameScreen.Acceleration.IsDown ? 1f : 0f;
            return new Vector2(horizontal, vertical);
        }

        private bool ReadFirePressedFromMobileUi()
        {
            return IsTouchSupported() && s.UI.GameScreen.Shoot.IsDown;
        }

        private Vector2 ReadMoveAxisFromDevices()
        {
            Vector2 axis = default;

            var keyboard = Keyboard.current;
            if (keyboard != null)
            {
                axis.x += keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed ? -1f : 0f;
                axis.x += keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed ? 1f : 0f;
                axis.y += keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed ? 1f : 0f;
                axis.y += keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed ? -1f : 0f;
            }

            var gamepad = Gamepad.current;
            if (gamepad != null && axis == Vector2.zero)
            {
                axis = gamepad.leftStick.ReadValue();
            }

            return Vector2.ClampMagnitude(axis, 1f);
        }

        private bool ReadFirePressedFromDevices()
        {
            var keyboard = Keyboard.current;
            if (keyboard != null && keyboard.spaceKey.wasPressedThisFrame)
            {
                return true;
            }

            var gamepad = Gamepad.current;
            return gamepad != null && gamepad.buttonSouth.wasPressedThisFrame;
        }

        private bool IsTouchSupported()
        {
            return Touchscreen.current != null;
        }
    }
}
