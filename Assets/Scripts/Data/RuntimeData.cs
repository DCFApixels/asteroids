using System;
using Asteroids.Utils;
using DCFApixels.DragonECS;
using UnityEngine;

namespace Asteroids.Data
{
    [Serializable]
    internal class RuntimeData
    {
        public Vector2 FieldSize;
        public AreaHash2D<entlong> AreaHash;
    }
}