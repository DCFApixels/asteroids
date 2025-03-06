using System.Collections.Generic;
using Asteroids.Components;
using Asteroids.Data;
using Asteroids.Utils;
using DCFApixels.DragonECS;

namespace Asteroids.Systems
{
    internal class CheckIntersectionWithAsteroidsSystem : IEcsRun
    {
        [DI] private EcsDefaultWorld _world;
        [DI] private RuntimeData _runtimeData;
        [DI] private StaticData _staticData;
        private readonly List<AreaHash2D<entlong>.Hit> _hits = new(64);

        private class Aspect : EcsAspect
        {
            public readonly EcsPool<RequestIntersection> RequestIntersections = Inc;
            public readonly EcsPool<TransformRef> Transform = Inc;
        }
    
        public void Run()
        {
            foreach (var e in _world.Where(out Aspect a))
            {
                var wantIntersection = a.RequestIntersections.Get(e);
                var position = a.Transform.Get(e).Value.position;
                _runtimeData.AreaHash.FindAllInRadius(position.x, position.z, wantIntersection.CheckRadius, _hits);

                foreach (var hit in _hits)
                {
                    var hitEntity = hit.Id;
                    if (!hitEntity.TryUnpack(out var entity, out EcsWorld ecsWorld))
                    {
                        continue;
                    }
                    var currentAsteroidRadius = _world.GetPool<Asteroid>().Get(entity).Radius + wantIntersection.ObjectRadius;

                    if (hit.SqrDistance <= currentAsteroidRadius * currentAsteroidRadius)
                    {
                        ecsWorld.GetPool<Hit>().TryAddOrGet(entity).ByObject = _world.GetEntityLong(e);
                        ecsWorld.GetPool<Hit>().TryAddOrGet(e).ByObject = hitEntity;
                        break;
                    }
                }
            }
        }
    }
}