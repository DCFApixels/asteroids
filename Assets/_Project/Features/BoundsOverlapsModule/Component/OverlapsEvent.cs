using DCFApixels.DragonECS;
using UnityEngine;

namespace Modules.BoundsOverlaps
{
    [MetaGroup(BoundsOverlapsModule.META_GROUP)]
    [MetaColor(BoundsOverlapsModule.META_COLOR)]
    [AllowedInWorlds("Graph", "Event")]
    [System.Serializable]
    public struct OverlapsEvent : IEcsComponent
    {
        public Vector3 Diff;
    }
}