using DCFApixels.DragonECS;
using UnityEngine;

namespace Asteroids.MovementFeature
{
    [System.Serializable]
    [MetaGroup(MovementModule.META_GROUP)]
    [MetaColor(MovementModule.META_COLOR)]
    [MetaID("986A2BB9950107332FCFF43280030220")]
    public struct Velocity : IEcsComponent
    {
        public Vector3 lineral;
        public Vector3 angular;
    }
}