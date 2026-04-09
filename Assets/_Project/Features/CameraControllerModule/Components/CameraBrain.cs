using DCFApixels.DragonECS;
using UnityEngine;
using UnityEngine.Serialization;

namespace Modules.CameraController
{
	[MetaGroup(CameraControllerModule.META_GROUP)]
	[MetaColor(CameraControllerModule.META_COLOR)]
	[System.Serializable]
	public class CameraBrain : MonoBehaviour
	{
		public Camera Camera;
		public Transform Pivod;
		public Transform ShakePivod;

		[FormerlySerializedAs("ShakeCurve")]
		public AnimationCurve ShakeSinCurve;
		public AnimationCurve ShakeOffsetCurve;
        public float ShakeSmooth = 0.5f;
        public Vector3 ShakeOffset = new Vector3(0, 0, 1);
		public Vector3 ShakeAmplitude = new Vector3(1, 1, 0);
        public Vector3 ShakePeriod = new Vector3(1, 1, 1);

        internal static float SoftSign(float x)
		{
			return x / (Mathf.Abs(x) + 1);
        }
        internal static Vector3 Sin(Vector3 a)
        {
			return new(Mathf.Sin(a.x), Mathf.Sin(a.y), Mathf.Sin(a.z));
        }
    }
}
