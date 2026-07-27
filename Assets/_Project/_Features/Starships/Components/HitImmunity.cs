using DCFApixels.DragonECS;

namespace Asteroids.StarshipsFeature
{
    [MetaGroup(StarshipsModule.META_GROUP, EcsConsts.COMPONENTS_GROUP)]
    [MetaColor(StarshipsModule.META_COLOR)]
    [System.Serializable]
    public struct HitImmunity : IEcsComponent
    {
        public float TimeLeft;
    }
}
