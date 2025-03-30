using DCFApixels.DragonECS;

namespace Asteroids.BoundsOverlapsFeature
{
    [MetaGroup(BoundsOverlapsModule.META_GROUP)]
    [MetaColor(BoundsOverlapsModule.META_COLOR)]
    [System.Serializable]
    public struct BoundsSphere : IEcsComponent
    {
        public float radius;
    }
}
