using Asteroids.Components;
using Asteroids.StarshipsFeature;
using DCFApixels.DragonECS;
using Modules.BoundsOverlaps;
using Modules.Motion;
using System.Linq;

namespace Asteroids.BulletsFeature
{
    class TouchToHitGameRuleSystem : IEcsRun, IEcsDefaultAddParams
    {
        public AddParams AddParams => -1;

        [DI] EntityGraph _graph;

        class RelAspect : EcsAspect
        {
            public EcsPool<OverlapsEvent> OverlapsEvents = Inc;
            public EcsPool<HitRequest> HitRequests = Exc;
            public EcsPool<HitImmunity> Immunities = Exc;
        }
        class BulletAspect : EcsAspect
        {
            public EcsPool<Velocity> Velocities = Inc;
            public EcsTagPool<TouchToHitEmmiter> TouchToHitEmmiters = Inc;
            public EcsPool<KillRequest> KillSignals = Exc;
        }
        class OtherAspect : EcsAspect
        {
            public EcsPool<HitImmunity> Immunities = Exc;
        }

        public void Run()
        {
            _graph.GraphWorld.GetAspects(out RelAspect relA);
            relA.HitRequests.ClearAll();
            var bulletEs = _graph.World.WhereToGroup(out BulletAspect bulletA);
            var otherEs = _graph.World.WhereToGroup(out OtherAspect otherA);

            foreach (var relE in _graph.GraphWorld.Where(relA))
            {
                var (bulletE, otehrE) = _graph.GetRelationStartEnd(relE);
                ref var overlaps = ref relA.OverlapsEvents[relE];
                if (bulletEs.Has(bulletE) && otherEs.Has(otehrE))
                {
                    ref var velocity = ref bulletA.Velocities[bulletE];

                    ref var hit = ref relA.HitRequests.Add(relE);
                    ref var immunity = ref relA.Immunities.Add(relE);
                    immunity.TimeLeft = 0.2f;
                    hit.DirectionNormal = velocity.Lineral.normalized;
                    hit.CollisionNormal = overlaps.Diff.normalized;
                }

            }
        }
    }
}
