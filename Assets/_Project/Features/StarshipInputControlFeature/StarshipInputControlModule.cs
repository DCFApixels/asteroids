using DCFApixels.DragonECS;

namespace Asteroids.StarshipInputControlFeature
{
    [MetaGroup(META_GROUP, EcsConsts.MODULES_GROUP)]
    [MetaColor(META_COLOR)]
    public class StarshipInputControlModule : IEcsModule, IEcsDefaultAddParams
    {
        public const string META_GROUP = nameof(Asteroids) + "." + nameof(StarshipInputControlFeature);
        public const uint META_COLOR = MetaColor.SkyBlue;
        public AddParams AddParams => META_GROUP;
        public void Import(EcsPipeline.Builder b)
        {
            b.Layers.InsertAfter(EcsConsts.BASIC_LAYER, META_GROUP);
            b.Add(new StarshipMovmentSystem());
        }
    }
}
