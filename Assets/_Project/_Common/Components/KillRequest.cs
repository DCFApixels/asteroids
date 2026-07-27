using DCFApixels.DragonECS;
using UnityEngine;
using Utils;

namespace Asteroids.Components
{
    [System.Serializable]
    public struct KillRequest : IEcsComponent
    {
        public Nlb<Vector3> Normal;
    }
}
