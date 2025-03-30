using Asteroids.BoundsOverlapsFeature;
using Asteroids.Components;
using Asteroids.Data;
using Asteroids.MovementFeature;
using Asteroids.Utils;
using DCFApixels.DragonECS;
using UnityEngine;

namespace Asteroids.Systems
{
    internal class CheckAsteroidHitSystem : IEcsRun
    {
        [DI] RuntimeData _runtimeData;
        [DI] StaticData _staticData;
        [DI] PoolService _poolService;
        [DI] EntityGraph _graph;

        class OtherAspect : EcsAspect
        {
            public EcsPool<TransformData> TransformDatas = Inc;
            public EcsPool<Asteroid> Asteroids = Exc;
        }
        class AsteroidAspect : EcsAspect
        {
            public EcsPool<Asteroid> Asteroids = Inc;
            public EcsPool<TransformData> TransformDatas = Inc;
            public EcsPool<Hit> Hits = Inc;
            public EcsPool<KillSignal> killSignals = Opt;
        }
        class RelAspect : EcsAspect
        {
            public EcsPool<OverlapsEvent> OverlapsEvents = Inc;
        }

        public void Run()
        {
            _graph.World.GetAspects(out OtherAspect otherA, out AsteroidAspect asteroidA);
            _graph.GraphWorld.GetAspects(out RelAspect relA);

            var map = _graph.GraphWorld.Where(relA).Join(JoinMode.All);
            foreach (var otherE in map.Nodes.Where(otherA))
            {
                ref var otherTransformData = ref otherA.TransformDatas[otherE];
                foreach (var relE in map.GetRelations(otherE))
                {
                    var asteroidE = _graph.GetRelationOpposite(relE, otherE);

                    ref var hit = ref asteroidA.Hits.TryAddOrGet(asteroidE);
                    hit.pointsSum += otherTransformData.position;
                    hit.pointsSumCount++;
                }
            }

            foreach (var asteroidE in _graph.World.Where(asteroidA))
            {
                ref var asteroid = ref asteroidA.Asteroids[asteroidE];
                ref var transformData = ref asteroidA.TransformDatas[asteroidE];
                ref var hit = ref asteroidA.Hits[asteroidE];

                var explosion = _poolService.Get(_staticData.AsteroidExplosionPrefab, out var instanceID);
                explosion.transform.position = transformData.position;
                explosion.Play(_poolService, instanceID);

                _runtimeData.Score++;
                asteroidA.killSignals.TryAddOrGet(asteroidE);

                if (asteroid.DeathsLeft <= 0)
                {
                    continue;
                }

                asteroid.DeathsLeft--;

                var forward = Vector3.forward;
                var hitPosition = hit.pointsSum / hit.pointsSumCount;
                if (hitPosition != transformData.position)
                {
                    forward = transformData.position - hitPosition;
                }
                var spawnPool = _graph.World.GetPool<SpawnAsteroidEvent>();
                for (var i = 0; i < 2; i++)
                {
                    var startForward = Quaternion.Euler(0, 90 + 180 * i, 0) * forward;

                    ref var spawnAsteroid = ref spawnPool.NewEntity();
                    spawnAsteroid.DeathsLeft = asteroid.DeathsLeft;
                    spawnAsteroid.Position = transformData.position;
                    spawnAsteroid.Rotation = Quaternion.LookRotation(startForward);
                    spawnAsteroid.StartRadius = asteroid.Radius / 2f;
                }
            }

            asteroidA.Hits.ClearAll();
        }

        private struct Hit : IEcsComponent
        {
            public Vector3 pointsSum;
            public int pointsSumCount;
        }
    }
}