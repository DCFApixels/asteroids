using UnityEngine;
using UnityEngine.InputSystem;

namespace Asteroids.LocalInputFeature
{
    [CreateAssetMenu(menuName = "Asteroids/Modules/Local Input Config")]
    public class LocalInputModuleConfig : ScriptableObject
    {
        public bool ShowMobileControlsOnTouchDevices = true;
        public InputActionReference MoveAction;
        public InputActionReference FireAction;
    }
}
