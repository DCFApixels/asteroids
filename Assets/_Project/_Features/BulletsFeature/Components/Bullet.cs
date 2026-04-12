using DCFApixels.DragonECS;

namespace Asteroids.BulletsFeature
{
    [System.Serializable]
    public struct Bullet : IEcsComponent
    {
        public BulletDescription Description;
    }
    [System.Serializable]
    public struct BulletLifeTime : IEcsComponent, IEcsTimerComponent
    {
        public float Time { get; set; }
    }
}