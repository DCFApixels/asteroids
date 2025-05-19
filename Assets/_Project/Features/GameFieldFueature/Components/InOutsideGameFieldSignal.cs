using DCFApixels.DragonECS;
using UnityEngine;

namespace Asteroids.GameFieldFueature
{
    [System.Serializable]
    internal struct InOutsideGameFieldSignal : IEcsComponent
    {
        public Vector3 Value;
    }
}
