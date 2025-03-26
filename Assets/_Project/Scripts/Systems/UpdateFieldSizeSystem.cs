using Asteroids.Data;
using DCFApixels;
using DCFApixels.DragonECS;
using UnityEngine;

namespace Asteroids.Systems
{
    internal class UpdateFieldSizeSystem : IEcsRun
    {
        [DI] private RuntimeData _runtimeData;
        [DI] private SceneData _sceneData;
        [DI] private StaticData _staticData;

        private float _prevAspect = -1f;
        public void Run()
        {
            var camera = _sceneData.Camera;
            Vector2 size;
            if (camera.orthographic)
            {
                if (Mathf.Approximately(_prevAspect, camera.aspect))
                {
                    return;
                }
                _prevAspect = camera.aspect;
                var orthographicSize = camera.orthographicSize;

                size = default;
                size.x = orthographicSize * camera.aspect * 2;
                size.y = orthographicSize * 2;
            }
            else
            {
                Plane gameFieldPlane = new Plane(Vector3.up, 0);
                gameFieldPlane.Raycast(new Ray(camera.transform.position, camera.transform.forward), out float distance);

                Vector3 bottomLeft = camera.ViewportToWorldPoint(new Vector3(0, 0, distance));
                Vector3 topRight = camera.ViewportToWorldPoint(new Vector3(1, 1, distance));
                size = new Vector2(topRight.x - bottomLeft.x, topRight.z - bottomLeft.z);
            }

            DebugX.Draw().WireQuad(Vector3.zero, Quaternion.LookRotation(Vector3.up), size);
            _runtimeData.FieldSize = size + Vector2.one * _staticData.ScreenBorderOffset;
            DebugX.Draw().WireQuad(Vector3.zero, Quaternion.LookRotation(Vector3.up), _runtimeData.FieldSize);
            _runtimeData.AreaHash = new(size.x / 4, -size.x / 2f, -size.y / 2f, size.x / 2f, size.y / 2f);
        }
    }
}