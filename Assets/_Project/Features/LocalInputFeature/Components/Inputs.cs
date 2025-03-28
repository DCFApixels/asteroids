using DCFApixels.DragonECS;
using UnityEngine;

namespace Asteroids.LocalInputFeature
{
    [System.Serializable]
    public struct MoveAxisInputSignal : IEcsComponent
    {
        public float Horizontal
        {
            get => Axis.x; 
            set => Axis.x = value;
        }
        public float Vertical
        {
            get => Axis.y;
            set => Axis.y = value;
        }
        public Vector2 Axis;
    }
    [System.Serializable]
    public struct FireInputBeginSignal : IEcsComponent { }
}