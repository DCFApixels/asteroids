using DCFApixels.DragonECS;
using UnityEngine;

namespace Asteroids.MovementFeature
{
    [MetaGroup(nameof(MovementModule), EcsConsts.COMPONENTS_GROUP)]
    [MetaColor(MetaColor.Cyan)]
    public struct TeleportSignal : IEcsComponent
    {
        public Vector3 newPosition;
        public Quaternion newRotation;
    }
    public class IsTeleportSignalTemplate : ComponentTemplate<TeleportSignal> { }
}
