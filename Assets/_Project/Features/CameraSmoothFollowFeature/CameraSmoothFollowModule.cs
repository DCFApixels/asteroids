using DCFApixels.DragonECS;

namespace Asteroids.CameraSmoothFollowFeature
{
    [MetaGroup(META_GROUP, EcsConsts.MODULES_GROUP)]
    [MetaColor(META_COLOR)]
    internal class CameraSmoothFollowModule : IEcsModule, IEcsDefaultAddParams
    {
        public const string META_GROUP = nameof(Asteroids) + "." + nameof(CameraSmoothFollowFeature);
        public const uint META_COLOR = MetaColor.BlueViolet;
        public AddParams AddParams => META_GROUP;
        public void Import(EcsPipeline.Builder b)
        {
            b.Layers.InsertAfter(EcsConsts.BASIC_LAYER, META_GROUP);
            b.Add(new CameraSmoothFollowSystem());
        }
    }
}
