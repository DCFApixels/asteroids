using Asteroids.Components;
using Asteroids.AsteroidsFeature;
using Asteroids.StarshipsFeature;
using DCFApixels.DragonECS;
using Modules.BoundsOverlaps;
using Modules.Motion;
using UnityEngine;

namespace Asteroids.BulletsFeature
{
    internal class BulletAsteroidCollisionSystem : IEcsRun, IEcsDefaultAddParams
    {
        public AddParams AddParams => -1;

        [DI] private EntityGraph _graph;

        private class RelationAspect : EcsAspect
        {
            public EcsPool<OverlapsEvent> Overlaps = Inc;
            public EcsPool<HitRequest> HitRequests = Opt;
            public EcsPool<HitImmunity> Immunities = Opt;
        }

        private class BulletAspect : EcsAspect
        {
            public EcsPool<Velocity> Velocities = Inc;
            public EcsTagPool<TouchToHitEmmiter> TouchToHitEmmiters = Inc;
            public EcsPool<KillRequest> KillRequests = Exc;
        }

        private class AsteroidAspect : EcsAspect
        {
            public EcsPool<Asteroid> Asteroids = Inc;
        }

        public void Run()
        {
            _graph.GraphWorld.GetAspects(out RelationAspect relationAspect);
            relationAspect.HitRequests.ClearAll();

            var bullets = _graph.World.WhereToGroup(out BulletAspect bulletAspect);
            var asteroids = _graph.World.WhereToGroup(out AsteroidAspect asteroidAspect);

            foreach (int relationEntity in _graph.GraphWorld.Where(relationAspect))
            {
                var (startEntity, endEntity) = _graph.GetRelationStartEnd(relationEntity);

                if (bullets.Has(startEntity) == false || asteroids.Has(endEntity) == false)
                {
                    continue;
                }

                if (relationAspect.Immunities.Has(relationEntity))
                {
                    continue;
                }

                Vector3 collisionNormal = relationAspect.Overlaps[relationEntity].Diff.normalized;
                ref Velocity bulletVelocity = ref bulletAspect.Velocities[startEntity];

                ref HitRequest hitRequest = ref relationAspect.HitRequests.TryAddOrGet(relationEntity);
                hitRequest.DirectionNormal = bulletVelocity.Lineral.normalized;
                hitRequest.CollisionNormal = collisionNormal;

                ref HitImmunity immunity = ref relationAspect.Immunities.TryAddOrGet(relationEntity);
                immunity.TimeLeft = 0.2f;
            }
        }
    }
}
