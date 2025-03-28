using DCFApixels.DragonECS;
using System;

namespace Asteroids.LocalInputFeature
{
    internal class LocalInputModule : IEcsModule
    {
        public void Import(EcsPipeline.Builder b)
        {
            b.Add(new LocalInputSystem());
        }
    }
}
