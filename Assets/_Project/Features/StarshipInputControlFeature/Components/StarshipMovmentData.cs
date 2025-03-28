using DCFApixels.DragonECS;

namespace Asteroids.StarshipInputControlFeature
{
    [System.Serializable]
    [MetaGroup(StarshipInputControlModule.META_GROUP)]
    [MetaColor(StarshipInputControlModule.META_COLOR)]
    [MetaID("89922AB99501C6637E5655E8ABB1F04A")]
    public struct StarshipMovmentData : IEcsComponent
    {
        public static readonly StarshipMovmentData Default = new StarshipMovmentData
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
