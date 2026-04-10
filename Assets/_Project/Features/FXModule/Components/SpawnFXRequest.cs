using DCFApixels.DragonECS;
using UnityEngine;

namespace Modules.FX
{
    [System.Serializable]
    internal struct SpawnFXRequest : IEcsComponent
    {
        public ScriptableEntityTemplateBase Template;
    }
    internal struct SpawnFXRequestPose : IEcsComponent
    {
        public Vector3 Position;
        public Quaternion Rotation;
    }
}