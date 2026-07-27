using DCFApixels.DragonECS;

namespace Asteroids.BulletsFeature
{
    [MetaGroup(BulletsModule.META_GROUP, EcsConsts.COMPONENTS_GROUP)]
    [MetaColor(BulletsModule.META_COLOR)]
    [System.Serializable]
    public struct Bullet : IEcsComponent
    {
        public BulletDescription Description;
    }
    [MetaGroup(BulletsModule.META_GROUP, EcsConsts.COMPONENTS_GROUP)]
    [MetaColor(BulletsModule.META_COLOR)]
    [System.Serializable]
    public struct BulletLifetime : IEcsComponent, IEcsTimerComponent
    {
        public float Time { get; set; }
    }
}
