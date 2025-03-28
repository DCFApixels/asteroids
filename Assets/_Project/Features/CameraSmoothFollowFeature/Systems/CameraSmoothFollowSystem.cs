using Asteroids.Data;
using Asteroids.MovementFeature;
using DCFApixels.DragonECS;
using System.Linq;
using UnityEngine;

namespace Asteroids.CameraSmoothFollowFeature
{
    internal class CameraSmoothFollowSystem : IEcsRun
    {
        [DI] EcsDefaultWorld _world;
        [DI] SceneData _sceneData;
        class Aspect : EcsAspect
        {
            public EcsPool<TransformData> TransformDatas = Inc;
            public EcsPool<CameraSmoothFollowTarget> cameraSmoothFollowTargets = Inc;
        }
        public void Run()
        {
            foreach (var e in _world.Where(out Aspect a))
            {
                ref var transformData = ref a.TransformDatas[e];
                ref var cameraSmoothFollowTarget = ref a.cameraSmoothFollowTargets[e];

                Vector3 campos = Vector3.Lerp(To2D(cameraSmoothFollowTarget.target), To2D(transformData.position), cameraSmoothFollowTarget.positionsLerp);
                campos.y = _sceneData.Camera.transform.position.y;

                campos = Vector3.Lerp(_sceneData.Camera.transform.position, campos, cameraSmoothFollowTarget.moveLerp);
                _sceneData.Camera.transform.position = campos;
            }
        }
        private static Vector3 To2D(Vector3 v)
        {
            v.y = 0;
            return v;
        }
    }
}
