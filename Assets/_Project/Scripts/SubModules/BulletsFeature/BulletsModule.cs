using DCFApixels.DragonECS;

namespace Asteroids.BulletsFeature
{
    internal class BulletsModule : IEcsModule
    {
        public void Import(EcsPipeline.Builder b)
        {
            b.Add(new BulletLifeTimeSystem());
        }
    }
}
