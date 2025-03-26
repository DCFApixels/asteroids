using DCFApixels.DragonECS;

namespace Asteroids.StartshipsFeature
{
    internal class StartshipsModule : IEcsModule
    {
        public void Import(EcsPipeline.Builder b)
        {
            b.Add(new StartshipDeathSystem());
        }
    }
}
