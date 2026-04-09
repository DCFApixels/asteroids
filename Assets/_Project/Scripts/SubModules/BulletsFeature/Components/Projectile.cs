using DCFApixels.DragonECS;

namespace Asteroids.BulletsFeature
{
    [System.Serializable]
    public struct Projectile : IEcsComponent
    {
        public ProjectileDescription Description;
    }
    [System.Serializable]
    public struct ProjectileLifetime : IEcsComponent, IEcsTimerComponent
    {
        public float Time { get; set; }
    }
}