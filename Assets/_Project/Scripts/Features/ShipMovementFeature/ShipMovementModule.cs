using DCFApixels.DragonECS;
using System;

namespace Asteroids.ShipMovementFeature
{
    [MetaGroup(META_GROUP, EcsConsts.MODULES_GROUP)]
    [MetaColor(META_COLOR)]
    public class ShipMovementModule : IEcsModule
    {
        public const string META_GROUP = nameof(ShipMovementModule);
        public const uint META_COLOR = MetaColor.SkyBlue;
        public void Import(EcsPipeline.Builder b)
        {
            b.Add(new ShipMovmentSystem());
        }
    }
}
