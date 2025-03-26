using DCFApixels.DragonECS;

namespace Asteroids.BulletsFeature
{
    [System.Serializable]
    public struct Bullet : IEcsComponent
    {
        public static readonly Bullet Instance = new Bullet()
        {
            lifeTimeMax = 3f,
        };
        public float lifeTimeMax;
        public float lifeTime;
    }
}