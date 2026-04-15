using DCFApixels.DragonECS;

namespace Modules.Motion
{
    [MetaGroup(MovementModule.META_GROUP)]
    [MetaColor(MovementModule.META_COLOR)]
    [MetaID("E2272BB99501499999DA540D0B04344B")]
    [System.Serializable]
    public struct VelocityDrag : IEcsComponent
    {
        public static readonly VelocityDrag Default = new VelocityDrag
        {
            LineralDrag = 0.05f,
            AngularDrag = 0.05f,
        };
        public float LineralDrag;
        public float AngularDrag;
    }
}