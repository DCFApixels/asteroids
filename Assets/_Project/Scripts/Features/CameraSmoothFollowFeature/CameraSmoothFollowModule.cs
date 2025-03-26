using DCFApixels.DragonECS;

namespace Asteroids.CameraSmoothFollowFeature
{
    internal class CameraSmoothFollowModule : IEcsModule
    {
        public void Import(EcsPipeline.Builder b)
        {
            b.Add(new CameraSmoothFollowSystem());
        }
    }
}
