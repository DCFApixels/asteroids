using DCFApixels.DragonECS;

namespace Asteroids.BulletsFeature
{
    [MetaGroup(BulletsModule.META_GROUP, EcsConsts.COMPONENTS_GROUP)]
    [MetaColor(BulletsModule.META_COLOR)]
    [System.Serializable]
    public struct TouchToHitEmitter : IEcsTagComponent
    {
    }
}
