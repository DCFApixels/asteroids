using Asteroids.Components;
using Asteroids.Data;
using DCFApixels.DragonECS;

namespace Asteroids.Systems
{
    internal class WrapAroundScreenSystem : IEcsRun
    {
        [DI] private EcsDefaultWorld _world;
        [DI] private RuntimeData _runtimeData;

        private class Aspect : EcsAspect
        {
            public readonly EcsPool<MoveInfo> MoveInfos = Inc;
            public readonly EcsPool<WrapAroundScreenMarker> WrapAroundScreenMarkers = Inc;
        }

        public void Run()
        {
            foreach (var e in _world.Where(out Aspect a))
            {
                ref var moveInfo = ref a.MoveInfos.Get(e);
                var marker = a.WrapAroundScreenMarkers.Get(e);

                var position = moveInfo.Position;
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

                moveInfo.Position = position;
            }
        }
    }
}