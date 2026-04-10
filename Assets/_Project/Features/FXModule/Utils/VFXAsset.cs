using UnityEngine;

namespace Modules.FX
{
    public class VFXAsset : MonoBehaviour
    {
        public float Duration = 1f;
        public ParticleSystem Root;
        public AudioSource AudioSource;
        public SFXAsset SFX;
        public void Play()
        {
            if (Root)
            {
                Root.Play();
            }
            if (AudioSource && SFX)
            {
                AudioSource.PlayOneShot(SFX);
            }
        }
    }
}