using DCFApixels.DragonECS;

namespace Modules.BoundsOverlaps
{
    [MetaGroup(META_GROUP, EcsConsts.MODULES_GROUP)]
    [MetaColor(META_COLOR)]
    public class BoundsOverlapsModule : IEcsModule, IEcsDefaultAddParams
    {
        public const string META_GROUP = nameof(Modules) + "/" + nameof(BoundsOverlaps);
        public const uint META_COLOR = MetaColor.Lime;
        public AddParams AddParams => META_GROUP;
        public void Import(EcsPipeline.Builder b)
        {
            b.Layers.Add(META_GROUP).After(EcsConsts.BASIC_LAYER);
            b.Add(new CheckShpereOverlapsSystem());
            b.Add(new DebugCheckShpereOverlapsSystem());
            b.Add(new RecalculateSpaceHashSystem());
        }
    }
    [System.Serializable]
    public class BoundsOverlapsRuntime
    {
        public AreaGrid2D<entlong> AreaGrid;
    }
}
