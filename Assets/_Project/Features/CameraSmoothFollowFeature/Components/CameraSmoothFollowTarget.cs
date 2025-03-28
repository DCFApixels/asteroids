using DCFApixels.DragonECS;
using UnityEngine;

namespace Asteroids.CameraSmoothFollowFeature
{
    [System.Serializable]
    public struct CameraSmoothFollowTarget : IEcsComponent
    {
        public Vector3 target;
        public float positionsLerp;
        public float moveLerp;
    }
}
