using DCFApixels.DragonECS;

namespace Asteroids.StarshipInputControlFeature
{
    [MetaGroup(StarshipInputControlModule.META_GROUP, EcsConsts.COMPONENTS_GROUP)]
    [MetaColor(StarshipInputControlModule.META_COLOR)]
    [MetaID("89922AB99501C6637E5655E8ABB1F04A")]
    [System.Serializable]
    public struct StarshipMovementData : IEcsComponent
    {
        public static readonly StarshipMovementData Default = new StarshipMovementData
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
}
