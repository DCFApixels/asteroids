using DCFApixels.DragonECS;

namespace Asteroids.GameFieldFeature
{
    [System.Serializable]
    [MetaGroup(GameFieldModule.META_GROUP, EcsConsts.COMPONENTS_GROUP)]
    [MetaColor(GameFieldModule.META_COLOR)]
    public struct OutOfGameFieldBehavior : IEcsComponent
    {
        public OutOfGameFieldBehaviorMode Mode;
    }
    public enum OutOfGameFieldBehaviorMode
    {
        Clamp,
        Wrap,
        Kill,
        KillImmediate,
    }
}
