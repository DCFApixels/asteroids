using DCFApixels.DragonECS;

namespace Asteroids.VFX
{
    [System.Serializable]
    internal struct VFXLifeTime : IEcsComponent, IEcsTimerComponent
    {
        public float Duration;
        public float Time;
        float IEcsTimerComponent.Time { get => Time; set => Time = value; }
    }
    [System.Serializable]
    internal struct VFXLifeTimeElapsedEvent : IEcsTagComponent { }
}