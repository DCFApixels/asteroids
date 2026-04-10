using DCFApixels.DragonECS;

namespace Modules.FX
{
    [System.Serializable]
    internal struct FXLifeTime : IEcsComponent, IEcsTimerComponent
    {
        public float Duration;
        public float Time;
        float IEcsTimerComponent.Time { get => Time; set => Time = value; }
    }
    [System.Serializable]
    internal struct FXLifeTimeElapsedEvent : IEcsTagComponent { }
}