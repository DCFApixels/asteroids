using DCFApixels.DragonECS;
using UnityEngine;
using Utils;

namespace Asteroids.Components
{
    [System.Serializable]
    public struct KillRequest : IEcsComponent
    {
        public KillRequestMode Mode;
        public Nlb<Vector3> Normal;
    }
    public enum KillRequestMode
    {
        Default,
        WithoutRestoring,
    }
}
