using DCFApixels.DragonECS;
using System;
using UnityEngine;

namespace Asteroids.MovementFeature
{
    [Serializable]
    [MetaGroup(nameof(MovementModule), EcsConsts.COMPONENTS_GROUP)]
    [MetaColor(MetaColor.Cyan)]
    public struct Velocity : IEcsComponent
    {
        public Vector3 lineral;
        public Vector3 angular;
    }
    public class VelocityTemplate : ComponentTemplate<Velocity> { }
}