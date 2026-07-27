using DCFApixels.DragonECS;

namespace Modules.CameraController
{
    public struct CameraShakeRequest : IEcsComponent
	{
		public float Strength;
		public float Duration;
    }
}
