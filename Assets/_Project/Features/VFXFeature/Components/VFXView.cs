using UnityEngine;

namespace Asteroids.Views
{
    public class VFXView : ViewBase
    {
        public ParticleSystem Root;
        public float Duration;

        public void Play()
        {
            Root.Play();
        }
    }
}