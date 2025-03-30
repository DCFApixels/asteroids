using Asteroids.BoundsOverlapsFeature;
using DCFApixels.DragonECS;
using System;
using UnityEngine;

namespace Asteroids.Data
{
    [Serializable]
    internal class RuntimeData
    {
        public GameState GameState;
        public int LifeLeft;
        public int Score;
        public Vector2 FieldSize;
        public AreaGrid2D<entlong> AreaGrid;
        public float LevelStartTime;
    }
}