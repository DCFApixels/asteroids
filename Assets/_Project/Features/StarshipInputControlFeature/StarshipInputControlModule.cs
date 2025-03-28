using DCFApixels.DragonECS;

namespace Asteroids.StarshipInputControlFeature
{
    [MetaGroup(META_GROUP, EcsConsts.MODULES_GROUP)]
    [MetaColor(META_COLOR)]
    public class StarshipInputControlModule : IEcsModule
    {
        public const string META_GROUP = nameof(StarshipInputControlModule);
        public const uint META_COLOR = MetaColor.SkyBlue;
        public void Import(EcsPipeline.Builder b)
        {
            b.Add(new StarshipMovmentSystem());
        }
    }
}
