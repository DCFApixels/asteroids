using Asteroids.AsteroidsFeature;
using Asteroids.Components;
using DCFApixels.DragonECS;
using Modules.BoundsOverlaps;
using Modules.Motion;
using UnityEngine;

namespace Asteroids.StarshipsFeature
{
    internal class StarshipAsteroidCollisionSystem : IEcsRun
    {
        [DI] private EntityGraph _graph;
        [DI] private RuntimeData _runtimeData;

        private class RelationAspect : EcsAspect
        {
            public EcsPool<OverlapsEvent> Overlaps = Inc;
        }

        private class StarshipAspect : EcsAspect
        {
            public EcsPool<Starship> Starships = Inc;
            public EcsPool<RigidTransform> Transforms = Inc;
            public EcsPool<HitImmunity> Immunities = Exc;
            public EcsPool<KillRequest> KillRequests = Opt;
        }

        private class AsteroidAspect : EcsAspect
        {
            public EcsPool<Asteroid> Asteroids = Inc;
            public EcsPool<RigidTransform> Transforms = Inc;
        }

        public void Run()
        {
            if (_runtimeData.GameState != GameState.Play)
            {
                return;
            }

            _graph.GraphWorld.GetAspects(out RelationAspect relationAspect);
            var starships = _graph.World.WhereToGroup(out StarshipAspect starshipAspect);
            var asteroids = _graph.World.WhereToGroup(out AsteroidAspect asteroidAspect);

            foreach (int relationEntity in _graph.GraphWorld.Where(relationAspect))
            {
                var (startEntity, endEntity) = _graph.GetRelationStartEnd(relationEntity);

                if (TryGetCollisionPair(startEntity, endEntity, starships, asteroids, out int starshipEntity, out int asteroidEntity) == false)
                {
                    continue;
                }

                ref KillRequest killRequest = ref starshipAspect.KillRequests.TryAddOrGet(starshipEntity);
                Vector3 normal = starshipAspect.Transforms[starshipEntity].Position - asteroidAspect.Transforms[asteroidEntity].Position;
                killRequest.Normal = normal.sqrMagnitude > 0.0001f ? normal.normalized : Vector3.forward;
            }
        }

        private static bool TryGetCollisionPair(
            int startEntity,
            int endEntity,
            EcsReadonlyGroup starships,
            EcsReadonlyGroup asteroids,
            out int starshipEntity,
            out int asteroidEntity)
        {
            if (starships.Has(startEntity) && asteroids.Has(endEntity))
            {
                starshipEntity = startEntity;
                asteroidEntity = endEntity;
                return true;
            }

            if (starships.Has(endEntity) && asteroids.Has(startEntity))
            {
                starshipEntity = endEntity;
                asteroidEntity = startEntity;
                return true;
            }

            starshipEntity = default;
            asteroidEntity = default;
            return false;
        }
    }
}
