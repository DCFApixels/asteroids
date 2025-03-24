using DCFApixels.DragonECS;
using UnityEngine;

namespace Asteroids.ControlsFeature
{
    [System.Serializable]
    [MetaGroup(ControlsModule.META_GROUP)]
    [MetaColor(ControlsModule.META_COLOR)]
    [MetaID("50752EB99501945F2BDC43F6F3BFA134")]
    public struct AxisControlData : IEcsComponent
    {
        public Vector3 Axis;
    }
    public class Template_50752EB99501945F2BDC43F6F3BFA134 : ComponentTemplate<AxisControlData> { }
}