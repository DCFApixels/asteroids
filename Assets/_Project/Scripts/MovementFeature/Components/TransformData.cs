using DCFApixels.DragonECS;
using System;
using UnityEngine;

namespace Asteroids.MovementFeature
{
    [Serializable]
    [MetaGroup(nameof(MovementModule), EcsConsts.COMPONENTS_GROUP)]
    [MetaColor(MetaColor.Cyan)]
    public struct TransformData : IEcsComponent
    {
        [Header("Current")]
        public Vector3 position;
        public Quaternion rotation;

        [Header("Last")]
        public Vector3 lastPosition;
        public Quaternion lastRotation;
    }
    public class TransformDataTemplate : ComponentTemplate<TransformData>
    {
        public TransformDataTemplate()
        {
            component.rotation = Quaternion.identity;
            component.lastRotation = Quaternion.identity;
        }
    }
}
