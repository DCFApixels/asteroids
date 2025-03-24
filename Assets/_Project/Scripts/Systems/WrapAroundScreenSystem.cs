using Asteroids.Components;
using Asteroids.Data;
using Asteroids.MovementFeature;
using DCFApixels.DragonECS;

namespace Asteroids.Systems
{
    internal class WrapAroundScreenSystem : IEcsRun
    {
        [DI] private EcsDefaultWorld _world;
        [DI] private RuntimeData _runtimeData;

        private class Aspect : EcsAspect
        {
            public readonly EcsPool<TransformData> TransformDatas = Inc;
            public readonly EcsPool<WrapAroundScreenMarker> WrapAroundScreenMarkers = Inc;
        }

        public void Run()
        {
            foreach (var e in _world.Where(out Aspect a))
            {
                ref var transformData = ref a.TransformDatas.Get(e);
                var marker = a.WrapAroundScreenMarkers.Get(e);

                var position = transformData.position;
                var fieldSize = _runtimeData.FieldSize;
                
                if (position.x < -fieldSize.x / 2f - marker.Offset)
                {
                    position.x += fieldSize.x + 2 * marker.Offset;
                }
                else
                {
                    if (position.x > fieldSize.x / 2f + marker.Offset)
                    {
                        position.x -= fieldSize.x + 2 * marker.Offset;
                    }
                }

                if (position.z < -fieldSize.y / 2f - marker.Offset)
                {
                    position.z += fieldSize.y + 2 * marker.Offset;
                }
                else
                {
                    if (position.z > fieldSize.y / 2f + marker.Offset)
                    {
                        position.z -= fieldSize.y + 2 * marker.Offset;
                    }
                }

                transformData.position = position;
            }
        }
    }
}