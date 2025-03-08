using Asteroids.Components;
using Asteroids.Data;
using DCFApixels.DragonECS;

namespace Asteroids.Systems
{
    internal class CycleAroundScreenSystem : IEcsRun
    {
        [DI] EcsDefaultWorld _world;
        [DI] RuntimeData _runtimeData;

        class Aspect : EcsAspect
        {
            public readonly EcsPool<TransformRef> TransformRefs = Inc;
            public readonly EcsPool<Cycle> CycleAroundScreens = Inc;
        }

        public void Run()
        {
            foreach (var e in _world.Where(out Aspect a))
            {
                var transform = a.TransformRefs.Get(e).Value;
                var cycle = a.CycleAroundScreens.Get(e);
                var position = transform.position;
                if (position.x < -_runtimeData.FieldSize.x / 2f - cycle.CycleOffset)
                {
                    position.x += _runtimeData.FieldSize.x + 2 * cycle.CycleOffset;
                }
                else
                {
                    if (position.x > _runtimeData.FieldSize.x / 2f + cycle.CycleOffset)
                    {
                        position.x -= _runtimeData.FieldSize.x + 2 * cycle.CycleOffset;
                    }
                }

                if (position.z < -_runtimeData.FieldSize.y / 2f - cycle.CycleOffset)
                {
                    position.z += _runtimeData.FieldSize.y + 2 * cycle.CycleOffset;
                }
                else
                {
                    if (position.z > _runtimeData.FieldSize.y / 2f + cycle.CycleOffset)
                    {
                        position.z -= _runtimeData.FieldSize.y + 2 * cycle.CycleOffset;
                    }
                }
                transform.position = position;
            }
        }
    }
}