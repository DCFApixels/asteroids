using System;
using Asteroids.Utils;
using DCFApixels.DragonECS;
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
        public AreaGrid2D<entlong> AreaHash;
        public float LevelStartTime;
    }
}