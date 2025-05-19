using DCFApixels.DragonECS;

namespace Asteroids.StartshipsFeature
{
    [System.Serializable]
    public struct HitImmunity : IEcsComponent
    {
        public float TimeLeft;
    }
}