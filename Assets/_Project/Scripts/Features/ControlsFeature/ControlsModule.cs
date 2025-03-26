using DCFApixels.DragonECS;

namespace Asteroids.ControlsFeature
{
    [MetaGroup(META_GROUP, EcsConsts.MODULES_GROUP)]
    [MetaColor(META_COLOR)]
    public class ControlsModule : IEcsModule
    {
        public const string META_GROUP = nameof(ControlsModule);
        public const uint META_COLOR = MetaColor.Blue;
        public void Import(EcsPipeline.Builder b)
        {
            b.Add(new InputToAxisControlSystem());
        }
    }
}