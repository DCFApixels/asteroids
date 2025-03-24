using DCFApixels.DragonECS;
using UnityEngine;

namespace Asteroids.ControlsFeature
{
    [MetaGroup(ControlsModule.META_GROUP)]
    [MetaColor(ControlsModule.META_COLOR)]
    public class InputToAxisControlSystem : IEcsRun
    {
        class Aspect : EcsAspect
        {
            public EcsPool<AxisControlData> axisControlDatas = Inc;
        }
        [DI] EcsDefaultWorld _world;
        public void Run()
        {
            foreach (var e in _world.Where(out Aspect a))
            {
                ref var axisControlData = ref a.axisControlDatas[e];
                axisControlData.Axis.x = Mathf.Sign(Input.GetAxis("Horizontal"));
                axisControlData.Axis.y = Mathf.Sign(Input.GetAxis("Vertical"));
            }
        }
    }
}
