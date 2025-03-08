using System;
using Unity.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Asteroids.Views
{
    internal class AsteroidView : MonoBehaviour
    {
        public LineRenderer LineRenderer;
        public float Radius = 2;
        
        public void SetRadius(float radius)
        {
            var points = new NativeArray<Vector3>(LineRenderer.positionCount, Allocator.Temp);
            LineRenderer.GetPositions(points);

            var angleStep = 2 * Mathf.PI / points.Length;
            
            for (var i = 0; i < points.Length; i++)
            {
                var angle = i * angleStep; 
                var x = Random.Range(radius/2f, radius) * Mathf.Cos(angle);
                var y = Random.Range(radius/2f, radius) * Mathf.Sin(angle);
                points[i] = new(x, y, 0);
            }
            
            LineRenderer.SetPositions(points);
            points.Dispose();
        }
    }
}

