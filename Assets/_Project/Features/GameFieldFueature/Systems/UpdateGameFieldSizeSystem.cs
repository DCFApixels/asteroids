using Asteroids.GameFieldFueature;
using DCFApixels;
using DCFApixels.DragonECS;
using UnityEngine;

namespace Asteroids.Systems
{
    [MetaGroup(GameFieldModule.META_GROUP)]
    [MetaColor(GameFieldModule.META_COLOR)]
    internal class UpdateGameFieldSizeSystem : IEcsRun, IEcsDefaultAddParams
    {
        public AddParams AddParams => EcsConsts.PRE_BEGIN_LAYER;

        [DI] RuntimeData r;
        [DI] SceneData s;
        [DI] ConfigData c;
        float _prevAspect = -1f;

        public void Run()
        {
            var camera = s.Camera;
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
            r.FieldSize = size + Vector2.one * c.ScreenBorderOffset;
            DebugX.Draw().WireQuad(Vector3.zero, Quaternion.LookRotation(Vector3.up), r.FieldSize);
            r.BoundsOverlapsRuntime.AreaGrid = new(size.x / 4, -size.x / 2f, -size.y / 2f, size.x / 2f, size.y / 2f);
        }
    }
}