using Asteroids.Components;
using DCFApixels.DragonECS;

namespace Asteroids.ControlsFeature
{
    [MetaGroup(ControlsModule.META_GROUP)]
    [MetaColor(ControlsModule.META_COLOR)]
    public class InputToAxisControlSystem : IEcsRun
    {
        class Aspect : EcsAspect
        {
            public EcsPool<AxisControlData> axisControlDatas = Inc;
            public EcsPool<InputData> InputDatas = Inc;
        }
        [DI] EcsDefaultWorld _world;
        public void Run()
        {
            foreach (var e in _world.Where(out Aspect a))
            {
                ref var axisControlData = ref a.axisControlDatas[e];
                ref var inputData = ref a.InputDatas[e];
                axisControlData.Axis.x = inputData.Horizontal;
                axisControlData.Axis.y = inputData.Vertical;
            }
        }
    }
}
