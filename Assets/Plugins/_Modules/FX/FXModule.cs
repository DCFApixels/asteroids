using DCFApixels.DragonECS;

namespace Modules.FX
{
    [MetaGroup(META_GROUP, EcsConsts.MODULES_GROUP)]
    [MetaColor(META_COLOR)]
    [System.Serializable]
    public class FXModule : IEcsModule, IEcsDefaultAddParams
    {
        public const string META_GROUP = nameof(Modules) + "/" + nameof(FX);
        public const uint META_COLOR = MetaColor.SkyBlue;
        public AddParams AddParams => META_GROUP;
        public void Import(EcsPipeline.Builder b)
        {
            b.Layers.Add(META_GROUP).After(EcsConsts.BASIC_LAYER);
            b.Add(new SpawnFXSystem());
            b.Add(new FXLifeTimeSystem());
            b.Add(new KillFXSystem());
        }
    }
}