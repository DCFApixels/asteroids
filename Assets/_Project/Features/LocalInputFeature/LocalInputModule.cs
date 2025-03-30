using DCFApixels.DragonECS;

namespace Asteroids.LocalInputFeature
{
    [MetaGroup(META_GROUP, EcsConsts.MODULES_GROUP)]
    [MetaColor(META_COLOR)]
    internal class LocalInputModule : IEcsModule, IEcsDefaultAddParams
    {
        public const string META_GROUP = nameof(Asteroids) + "." + nameof(LocalInputFeature);
        public const uint META_COLOR = MetaColor.Cyan;
        public AddParams AddParams => META_GROUP;
        public void Import(EcsPipeline.Builder b)
        {
            b.Layers.InsertAfter(EcsConsts.PRE_BEGIN_LAYER, META_GROUP);
            b.Add(new LocalInputSystem());
        }
    }
}
