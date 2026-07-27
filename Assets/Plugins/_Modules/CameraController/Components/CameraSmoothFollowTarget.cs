using DCFApixels.DragonECS;
using UnityEngine;

namespace Modules.CameraController
{
    [MetaGroup(CameraControllerModule.META_GROUP)]
    [MetaColor(CameraControllerModule.META_COLOR)]
    [System.Serializable]
    public struct CameraSmoothFollowTarget : IEcsComponent
    {
        public static readonly CameraSmoothFollowTarget Default = new()
        {
            PositionsLerp = 0.03f,
            MoveLerp = 0.02f,
        };
        public Vector3 Target;
        public float PositionsLerp;
        public float MoveLerp;
    }
}
