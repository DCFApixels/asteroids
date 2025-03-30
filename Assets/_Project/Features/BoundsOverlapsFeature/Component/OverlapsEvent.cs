using DCFApixels.DragonECS;

namespace Asteroids.BoundsOverlapsFeature
{
    [MetaGroup(BoundsOverlapsModule.META_GROUP)]
    [MetaColor(BoundsOverlapsModule.META_COLOR)]
    [AllowedInWorlds("Graph", "Event")]
    [System.Serializable]
    internal struct OverlapsEvent : IEcsComponent { }
}