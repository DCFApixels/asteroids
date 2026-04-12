using UnityEngine;

namespace Modules.FX
{
    public class ShortVFXView : MonoBehaviour
    {
        public float Duration = 1f;
        public ParticleSystem Root;
        public AudioSource AudioSource;
        public SoundEffect SoundEffect;
        public float ScaleBlend = 0.5f;

        public ParticleSystem[] ColoredParticles = new ParticleSystem[0];

        public void Play(float? scale, Color? color)
        {
            if(scale != null)
            {
                transform.localScale = Vector3.Lerp(Vector3.one, Vector3.one * scale.Value, ScaleBlend);
            }
            if (AudioSource && SoundEffect)
            {
                var clip = SoundEffect.GetRandomClip();
                if(scale != null)
                {
                    clip.Volume *= Mathf.Lerp(1f, scale.Value, ScaleBlend);
                   // clip.Pitch *= 1f / Mathf.Lerp(1f, scale.Value, ScaleBlend);
                }
                AudioSource.PlayOneShot(clip);
            }
            if (color != null)
            {
                foreach (var p in ColoredParticles)
                {
                    var main = p.main;
                    main.startColor = color.Value;
                }
            }

            if (Root)
            {
                Root.Play();
            }
        }
    }
}