using Asteroids.Components;
using Asteroids.Data;
using Asteroids.MovementFeature;
using Asteroids.StartshipsFeature;
using Asteroids.Utils;
using DCFApixels.DragonECS;
using System.Collections.Generic;

namespace Asteroids.Systems
{
    internal class CheckIntersectionWithAsteroidsSystem : IEcsRun
    {
        [DI] EcsDefaultWorld _world;
        [DI] RuntimeData _runtimeData;

        List<AreaGrid2D<entlong>.Hit> _hits = new(64);

        private class Aspect : EcsAspect
        {
            public EcsPool<RequestIntersectionEvent> RequestIntersectionEvents = Inc;
            public EcsPool<TransformData> TransformDatas = Inc;
        }

        public void Run()
        {
            EcsPool<OverlapsEvent> hitEvents = _world.GetPool<OverlapsEvent>();
            
            foreach (var e in _world.Where(out Aspect a))
            {
                ref var wantIntersection = ref a.RequestIntersectionEvents[e];
                var position = a.TransformDatas[e].position;
                _runtimeData.AreaHash.FindAllInRadius(position.x, position.z, wantIntersection.CheckRadius, _hits);

                foreach (var hit in _hits)
                {
                    var hitEntity = hit.Id;
                    if (!hitEntity.TryUnpack(out var entity, out EcsWorld world) && world != _world)
                    {
                        continue;
                    }
                    var currentAsteroidRadius = _world.GetPool<Asteroid>().Get(entity).Radius + wantIntersection.ObjectRadius;

                    if (hit.SqrDistance <= currentAsteroidRadius * currentAsteroidRadius)
                    {
                        hitEvents.TryAddOrGet(entity).largestEntity = _world.GetEntityLong(e);
                        if (!_world.GetPool<Immunity>().Has(e))
                        {
                            hitEvents.TryAddOrGet(e).largestEntity = hitEntity;
                        }
                        break;
                    }
                }
            }
        }
    }
}