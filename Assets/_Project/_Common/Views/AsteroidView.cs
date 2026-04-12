using TriInspector;
using Unity.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Asteroids.Views
{
    public class AsteroidView : ViewBase
    {
        public LineRenderer LineRenderer;
        public Transform Light;
        public float Radius = 2;

        public override float GetScale()
        {
            return Radius;
        }
        public void SetRadius(float radius)
        {
            LineRenderer.transform.localPosition = Vector3.zero;

            var points = new NativeArray<Vector3>(LineRenderer.positionCount, Allocator.Temp);
            LineRenderer.GetPositions(points);

            Radius = radius;

            var centroid = Vector3.zero;
            var angleStep = 2 * Mathf.PI / points.Length;
            var quarterRadius = radius * 0.25f;
            for (var i = 0; i < points.Length; i++)
            {
                var angle = i * angleStep;
                var x = Random.Range(radius * 0.8f, radius * 1.5f) * Mathf.Cos(angle);
                var y = Random.Range(radius * 0.8f, radius * 1.5f) * Mathf.Sin(angle);
                points[i] = new(x, y, 0);
                centroid += new Vector3(x, y, 0);
            }
            centroid /= points.Length;

            if (centroid.sqrMagnitude > 0.0001f)
            {
                LineRenderer.transform.localPosition = -centroid;
            }

            LineRenderer.SetPositions(points);
            points.Dispose();

            Light.localScale = Vector3.one * radius;
        }

        [Button]
        protected void ApplyRadius()
        {
            SetRadius(Radius);
        }
    }
}

