using UnityEngine;

namespace Asteroids.Views
{
    public class StarshipView : MonoBehaviour
    {
        public LineRenderer LineRenderer;
        public Color MinColor;
        public Color MaxColor;
        public float BlinkFrequency;
        
        public float Radius;

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position, Radius);
        }
        public void BlinkFromValueReset()
        {
            LineRenderer.startColor = LineRenderer.endColor = MinColor;
        }
        public void BlinkFromValue(float value)
        {
            var a = Mathf.Sin(value * BlinkFrequency) / 2f + 0.5f;
            var color = Color.Lerp(MinColor, MaxColor, a);
            LineRenderer.startColor = LineRenderer.endColor = color;
        }
    }
}