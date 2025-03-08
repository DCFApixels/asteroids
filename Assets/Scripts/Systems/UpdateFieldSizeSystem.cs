using Asteroids.Data;
using DCFApixels.DragonECS;
using UnityEngine;

namespace Asteroids.Systems
{
    internal class UpdateFieldSizeSystem : IEcsRun
    {
        [DI] private RuntimeData _runtimeData;
        [DI] private SceneData _sceneData;

        private float _prevAspect = -1f;
        public void Run()
        {
            if (Mathf.Approximately(_prevAspect, _sceneData.Camera.aspect))
            {
                return;
            }
            
            _prevAspect = _sceneData.Camera.aspect;
            var orthographicSize = _sceneData.Camera.orthographicSize;

            var size = _runtimeData.FieldSize;
            size.x = orthographicSize * _sceneData.Camera.aspect * 2;
            size.y = orthographicSize * 2;
            _runtimeData.FieldSize = size;
            
            _runtimeData.AreaHash = new (size.x / 4, -size.x/2f,-size.y/2f, size.x/2f,size.y/2f);
        }
    }
}