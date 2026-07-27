using Asteroids.AsteroidsFeature;
using Asteroids.Components;
using DCFApixels.DragonECS;
using Modules.BoundsOverlaps;
using Modules.Motion;
using UnityEngine;

namespace Asteroids.StarshipsFeature
{
    [MetaGroup(StarshipsModule.META_GROUP, EcsConsts.SYSTEMS_GROUP)]
    [MetaColor(StarshipsModule.META_COLOR)]
    class StarshipAsteroidCollisionSystem : IEcsRun
    {
        [DI] EntityGraph _graph;
        [DI] GameRuntimeData _gameRuntime;

        class RelationAspect : EcsAspect
        {
            public EcsPool<OverlapsEvent> Overlaps = Inc;
        }

        class StarshipAspect : EcsAspect
        {
            public EcsPool<Starship> Starships = Inc;
            public EcsPool<RigidTransform> RigidTransforms = Inc;
            public EcsPool<HitImmunity> Immunities = Exc;
            public EcsPool<KillRequest> KillRequests = Opt;
        }

        class AsteroidAspect : EcsAspect
        {
            public EcsPool<Asteroid> Asteroids = Inc;
            public EcsPool<RigidTransform> RigidTransforms = Inc;
        }

        public void Run()
        {
            if (_gameRuntime.GameState != GameState.Play)
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
                Vector3 normal = starshipAspect.RigidTransforms[starshipEntity].Position - asteroidAspect.RigidTransforms[asteroidEntity].Position;
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
