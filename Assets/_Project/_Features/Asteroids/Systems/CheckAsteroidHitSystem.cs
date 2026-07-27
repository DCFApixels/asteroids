using Asteroids.Components;
using DCFApixels.DragonECS;
using Modules.BoundsOverlaps;
using Modules.CameraController;
using Modules.Motion;
using UnityEngine;

namespace Asteroids.AsteroidsFeature
{
    [MetaGroup(AsteroidsModule.META_GROUP, EcsConsts.SYSTEMS_GROUP)]
    [MetaColor(AsteroidsModule.META_COLOR)]
    class CheckAsteroidHitSystem : IEcsRun
    {
        [DI] GameRuntimeData _gameRuntime;
        [DI] AsteroidsFeatureConfig _config;
        [DI] EntityGraph _graph;

        class OtherAspect : EcsAspect
        {
            public EcsPool<Asteroid> Asteroids = Exc;
        }

        class RelAspect : EcsAspect
        {
            public EcsPool<HitRequest> HitRequests = Inc;
        }

        class AsteroidAspect : EcsAspect
        {
            public EcsPool<Asteroid> Asteroids = Inc;
            public EcsPool<BoundsSphere> BoundsSpheres = Inc;
            public EcsPool<RigidTransform> RigidTransforms = Inc;
            public EcsPool<KillRequest> KillRequests = Opt;
        }

        class HitSourceAspect : EcsAspect
        {
            public EcsPool<Asteroid> Asteroids = Exc;
            public EcsPool<KillRequest> KillRequests = Opt;
        }

        public void Run()
        {
            _graph.World.GetAspects(out AsteroidAspect asteroidA, out OtherAspect otherA);
            var hitSources = _graph.World.WhereToGroup(out HitSourceAspect hitSourceA);
            _graph.GraphWorld.GetAspects(out RelAspect relA);

            var map = _graph.GraphWorld.Where(relA).Join(JoinMode.End);
            foreach (var asteroidE in map.Nodes.Where(asteroidA))
            {
                Vector3 hitNormal = default;
                int hitCount = 0;

                foreach (var relE in map.GetRelations(asteroidE))
                {
                    var otherE = _graph.GetRelationOpposite(relE, asteroidE);
                    if (_graph.World.IsMatchesMask(otherA, otherE) == false)
                    {
                        continue;
                    }

                    ref var hitRequest = ref relA.HitRequests[relE];
                    hitNormal += hitRequest.DirectionNormal;
                    hitCount++;

                    if (hitSources.Has(otherE))
                    {
                        ref var killRequest = ref hitSourceA.KillRequests.TryAddOrGet(otherE);
                        killRequest.Normal = hitRequest.CollisionNormal;
                    }
                }

                if (hitCount == 0)
                {
                    continue;
                }

                hitNormal = hitNormal.sqrMagnitude > 0.0001f ? hitNormal.normalized : Vector3.forward;

                ref var asteroid = ref asteroidA.Asteroids[asteroidE];
                ref var boundsSphere = ref asteroidA.BoundsSpheres[asteroidE];
                ref var rigidTransform = ref asteroidA.RigidTransforms[asteroidE];

                _gameRuntime.Score++;
                asteroidA.KillRequests.TryAddOrGet(asteroidE);
                ShakeCamera();

                if (asteroid.DeathsLeft <= 0)
                {
                    continue;
                }

                asteroid.DeathsLeft--;
                SpawnFragments(asteroid, boundsSphere.Radius, rigidTransform.Position, hitNormal);
            }
        }

        void SpawnFragments(Asteroid asteroid, float parentRadius, Vector3 position, Vector3 hitNormal)
        {
            var requestsPool = _graph.World.GetPool<SpawnAsteroidRequest>();
            for (var i = 0; i < 2; i++)
            {
                var startForward = Quaternion.Euler(0, 90 + 180 * i, 0) * hitNormal;

                ref var req = ref requestsPool.NewEntity();
                req.Description = asteroid.Description;
                req.OverrideRadius = parentRadius * _config.AsteroidSplitMultiplier;
                req.OverrideDeathsCount = asteroid.DeathsLeft;
                req.Position = position;
                req.Rotation = Quaternion.LookRotation(startForward);
            }
        }

        void ShakeCamera()
        {
            ref var shake = ref _graph.World.GetPool<CameraShakeRequest>().NewEntity();
            shake.Strength = 1f;
            shake.Duration = 0.24f;
        }
    }
}
