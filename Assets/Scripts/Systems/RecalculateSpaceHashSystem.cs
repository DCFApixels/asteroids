using Asteroids.Components;
using Asteroids.Data;
using DCFApixels.DragonECS;

namespace Asteroids.Systems
{
    internal class RecalculateSpaceHashSystem : IEcsRun
    {
        [DI] private RuntimeData _runtimeData;
        [DI] private EcsDefaultWorld _world;

        class AsteroidAspect : EcsAspect
        {
            public readonly EcsPool<TransformRef> TransformRefs = Inc;
            public EcsPool<Asteroid> Asteroids = Inc;
        }
        public void Run()
        {
            _runtimeData.AreaHash.Clear();

            foreach (var e in _world.Where(out AsteroidAspect a))
            {
                var transform = a.TransformRefs.Get(e).Value;
                _runtimeData.AreaHash.Add(_world.GetEntityLong(e), transform.position.x, transform.position.z);
            }
        }
    }
}