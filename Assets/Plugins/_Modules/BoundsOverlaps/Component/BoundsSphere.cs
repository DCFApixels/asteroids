using DCFApixels.DragonECS;

namespace Modules.BoundsOverlaps
{
    [MetaGroup(BoundsOverlapsModule.META_GROUP)]
    [MetaColor(BoundsOverlapsModule.META_COLOR)]
    [System.Serializable]
    public struct BoundsSphere : IEcsComponent
    {
        public static readonly BoundsSphere Default = new BoundsSphere
        {
            Radius = 1f,
        };
        public float Radius;
    }
}
