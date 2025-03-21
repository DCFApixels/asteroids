using DCFApixels.DragonECS;
using System;

namespace Asteroids.MovementFeature
{
    [Serializable]
    [MetaGroup(nameof(MovementModule), EcsConsts.COMPONENTS_GROUP)]
    [MetaColor(MetaColor.Cyan)]
    public struct RigidbodyData : IEcsComponent
    {
        public float lineralDrag;
        public float angularDrag;

        public float mass;
    }
    public class RigidbodyDataTemplate : ComponentTemplate<RigidbodyData> { }
}