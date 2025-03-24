using DCFApixels.DragonECS;

namespace Asteroids.ShipMovementFeature
{
    [System.Serializable]
    [MetaGroup(ShipMovementModule.META_GROUP)]
    [MetaColor(ShipMovementModule.META_COLOR)]
    [MetaID("89922AB99501C6637E5655E8ABB1F04A")]
    public struct ShipMovementData : IEcsComponent
    {
        public static readonly ShipMovementData Default = new ShipMovementData
        {
            MaxSpeed = 10,
            Acceleration = 10,
            MaxRotationSpeed = 360
        };
        public float MaxSpeed;
        public float MaxRotationSpeed;
        public float Acceleration;

        public float Power;

    }
    public class Template_89922AB99501C6637E5655E8ABB1F04A : ComponentTemplate<ShipMovementData> { }
}
