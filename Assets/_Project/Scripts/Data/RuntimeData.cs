﻿using Asteroids.Utils;
using DCFApixels.DragonECS;
using System;
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