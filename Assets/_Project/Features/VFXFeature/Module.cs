using DCFApixels.DragonECS;

namespace Asteroids.VFX
{
    [MetaGroup(META_GROUP, EcsConsts.MODULES_GROUP)]
    [MetaColor(META_COLOR)]
    public class VFXModule : IEcsModule, IEcsDefaultAddParams
    {
        public const string META_GROUP = nameof(Asteroids) + "/" + nameof(VFX);
        public const uint META_COLOR = MetaColor.SkyBlue;
        public AddParams AddParams => META_GROUP;
        public void Import(EcsPipeline.Builder b)
        {
            b.Layers.Add(META_GROUP).After(EcsConsts.BASIC_LAYER);
            b.Add(new VFXLifeTimeSystem());
            b.Add(new KillVFXSystem());
        }
    }
}