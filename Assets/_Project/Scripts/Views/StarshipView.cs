using UnityEngine;
using Color = UnityEngine.Color;

namespace Asteroids.Views
{
    public class StarshipView : MonoBehaviour
    {
        public LineRenderer LineRenderer;
        public Color MinColor;
        public Color MaxColor;
        public float BlinkFrequency;
        
        public float Radius;

        private Awaitable _routine;

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position, Radius);
        }

        public void BlinkFromValue(float value)
        {
            var a = Mathf.Sin(value * BlinkFrequency) / 2f + 0.5f;
            SetColor(Color.Lerp(MinColor, MaxColor, a));
        }
        private void SetColor(Color color)
        {
            LineRenderer.startColor = LineRenderer.endColor = color;
        }


        public void BlinkFromValueReset()
        {
            if (_routine != null && _routine.IsCompleted == false)
            {
                _routine.Cancel();
            }
            _routine = ColorAnimation(LineRenderer.startColor, MinColor, 0.2f);
        }
        private async Awaitable ColorAnimation(Color startColor, Color endColor, float duration)
        {
            float time = duration;
            while (true)
            {
                time -= Time.deltaTime;
                SetColor(Color.Lerp(endColor, startColor, time / duration));
                if (time <= 0)
                {
                    break;
                }
                await Awaitable.NextFrameAsync();
            }
        }
    }
}