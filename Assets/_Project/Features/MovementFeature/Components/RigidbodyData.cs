using DCFApixels.DragonECS;

namespace Asteroids.MovementFeature
{
    [MetaGroup(MovementModule.META_GROUP)]
    [MetaColor(MovementModule.META_COLOR)]
    [MetaID("E2272BB99501499999DA540D0B04344B")]
    [System.Serializable]
    public struct RigidbodyData : IEcsComponent
    {
        public static readonly RigidbodyData Default = new RigidbodyData
        {
            mass = 1,
            lineralDrag = 0.05f,
            angularDrag = 0.05f,
        };
        public float lineralDrag;
        public float angularDrag;

        public float mass;
    }
}