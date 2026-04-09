using DCFApixels.DragonECS;

namespace Modules.CameraController
{
    [MetaGroup(META_GROUP, EcsConsts.MODULES_GROUP)]
    [MetaColor(META_COLOR)]
    public class CameraControllerModule : IEcsModule, IEcsDefaultAddParams
    {
        public const string META_GROUP = nameof(Modules) + "/" + nameof(CameraController);
        public const uint META_COLOR = MetaColor.BlueViolet;
        public AddParams AddParams => META_GROUP;
        public void Import(EcsPipeline.Builder b)
        {
            b.Layers.Add(META_GROUP).After(EcsConsts.BASIC_LAYER);
            b.Add(new CameraSmoothFollowSystem());
            b.Add(new CameraShakeSystem());
        }
    }
}
