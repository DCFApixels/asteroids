using DCFApixels.DragonECS;
using UnityEngine;

namespace Asteroids.Components
{
    [AllowedInWorlds("Graph", "Event")]
    [System.Serializable]
    public struct HitRequest : IEcsComponent
    {
        public Vector3 directionNormal;
    }

    [AllowedInWorlds("Graph", "Event")]
    [System.Serializable]
    public struct HitAnswer : IEcsComponent
    {
        public Vector3 directionNormal;
    }
}
