using DCFApixels.DragonECS;

namespace Modules.BoundsOverlaps
{
    [MetaGroup(BoundsOverlapsModule.META_GROUP)]
    [MetaColor(BoundsOverlapsModule.META_COLOR)]
    [AllowedInWorlds("Graph", "Event")]
    [System.Serializable]
    public struct OverlapsEvent : IEcsComponent { }
}