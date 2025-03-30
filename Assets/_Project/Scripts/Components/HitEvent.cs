using DCFApixels.DragonECS;
using System.Numerics;

namespace Asteroids.Components
{
    [AllowedInWorlds("Graph", "Event")]
    [System.Serializable]
    public struct HitEvent : IEcsComponent
    {
        public Vector3 directionNormal;
    }
}
