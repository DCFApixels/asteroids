using DCFApixels.DragonECS;

namespace Modules.Movement
{
    [MetaGroup(MovementModule.META_GROUP)]
    [MetaColor(MovementModule.META_COLOR)]
    [MetaID("E2272BB99501499999DA540D0B04344B")]
    [System.Serializable]
    public struct RigidbodyData : IEcsComponent
    {
        public static readonly RigidbodyData Default = new RigidbodyData
        {
            Mass = 1,
            LineralDrag = 0.05f,
            AngularDrag = 0.05f,
        };
        public float LineralDrag;
        public float AngularDrag;

        public float Mass;
    }
}