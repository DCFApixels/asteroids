using Asteroids.Systems;
using DCFApixels.DragonECS;

namespace Asteroids.GameFieldFueature
{
    [MetaGroup(META_GROUP, EcsConsts.MODULES_GROUP)]
    [MetaColor(META_COLOR)]
    internal class GameFieldModule : IEcsModule, IEcsDefaultAddParams
    {
        public const string META_GROUP = nameof(Asteroids) + "." + nameof(GameFieldFueature);
        public const uint META_COLOR = MetaColor.BlueViolet;
        public AddParams AddParams => META_GROUP;
        public void Import(EcsPipeline.Builder b)
        {
            b.Layers.Insert(EcsConsts.END_LAYER, META_GROUP);
            b.Add(new UpdateFieldSizeSystem());
            b.Add(new CheckAroundGameFieldSystem());
            b.Add(new WrapAroundGameFieldSystem());
            b.Add(new KillOutsideGameFieldSystem());
        }
    }
}
