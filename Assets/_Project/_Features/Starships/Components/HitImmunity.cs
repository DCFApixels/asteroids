using DCFApixels.DragonECS;

namespace Asteroids.StarshipsFeature
{
    [System.Serializable]
    public struct HitImmunity : IEcsComponent
    {
        public float TimeLeft;
    }
}