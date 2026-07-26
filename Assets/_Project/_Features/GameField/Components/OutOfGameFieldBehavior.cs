using DCFApixels.DragonECS;

namespace Asteroids.GameFieldFeature
{
    [System.Serializable]
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