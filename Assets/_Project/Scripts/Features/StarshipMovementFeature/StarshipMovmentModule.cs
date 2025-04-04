﻿using DCFApixels.DragonECS;
using System;

namespace Asteroids.StarshipMovmentFeature
{
    [MetaGroup(META_GROUP, EcsConsts.MODULES_GROUP)]
    [MetaColor(META_COLOR)]
    public class StarshipMovmentModule : IEcsModule
    {
        public const string META_GROUP = nameof(StarshipMovmentModule);
        public const uint META_COLOR = MetaColor.SkyBlue;
        public void Import(EcsPipeline.Builder b)
        {
            b.Add(new StarshipMovmentSystem());
        }
    }
}
