using DCFApixels.DragonECS;

namespace Asteroids.GameFieldFueature
{
    internal class GameFieldModule : IEcsModule
    {
        public void Import(EcsPipeline.Builder b)
        {
            b.Add(new CheckAroundGameFieldSystem());
            b.Add(new WrapAroundGameFieldSystem());
            b.Add(new KillOutsideGameFieldSystem());
        }
    }
}
