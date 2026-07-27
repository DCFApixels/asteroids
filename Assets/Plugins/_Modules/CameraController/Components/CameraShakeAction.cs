using DCFApixels.DragonECS;

namespace Modules.CameraController
{
    internal struct CameraShakeAction : IEcsComponent, IEcsTimerComponent
    {
        public float Strength;
        public float Duration;
        public float Time { get; set; }

        public float T
        {
            get
            {
                return 1 - Time / Duration;
            }
        }
    }
}
