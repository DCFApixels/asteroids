using DCFApixels.DragonECS;

namespace Asteroids.BoundsOverlapsFeature
{
    [MetaGroup(META_GROUP, EcsConsts.MODULES_GROUP)]
    [MetaColor(META_COLOR)]
    class BoundsOverlapsModule : IEcsModule, IEcsDefaultAddParams
    {
        public const string META_GROUP = nameof(Asteroids) + "." + nameof(BoundsOverlapsFeature);
        public const uint META_COLOR = MetaColor.Lime;
        public AddParams AddParams => META_GROUP;
        public void Import(EcsPipeline.Builder b)
        {
            b.Layers.InsertAfter(EcsConsts.BEGIN_LAYER, META_GROUP);
            b.Add(new CheckShpereOverlapsSystem());
            b.Add(new RecalculateSpaceHashSystem());
        }
    }
}
