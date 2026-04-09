using DCFApixels.DragonECS;
using UnityEngine;

namespace Asteroids.MovementFeature
{
    [MetaGroup(MovementModule.META_GROUP)]
    [MetaColor(MovementModule.META_COLOR)]
    [MetaID("986A2BB9950107332FCFF43280030220")]
    [System.Serializable]
    public struct Velocity : IEcsComponent
    {
        public Vector3 Lineral;
        public Vector3 Angular;
    }
}