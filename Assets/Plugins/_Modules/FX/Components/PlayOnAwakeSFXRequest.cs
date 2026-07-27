using DCFApixels.DragonECS;
using UnityEngine;

namespace Modules.FX
{
    [System.Serializable]
    internal struct PlayOnAwakeSFXRequest : IEcsComponent, IEcsTimerComponent
    {
        public SoundEffect SFX;
        public AudioSource AudioSourceOverride;
        public float Delay;
        float IEcsTimerComponent.Time { get => Delay; set => Delay = value; }
    }
}