using Asteroids.BoundsOverlapsFeature;
using Asteroids.Components;
using Asteroids.MovementFeature;
using Asteroids.StartshipsFeature;
using DCFApixels.DragonECS;
using System.Linq;

namespace Asteroids.BulletsFeature
{
    class TouchToHitRequestSystem : IEcsRun, IEcsDefaultAddParams
    {
        [DI] EntityGraph _graph;

        public AddParams AddParams => -1;

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
            public EcsPool<KillSignal> KillSignals = Exc;
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
                if (bulletEs.Has(bulletE) &&
                    otherEs.Has(otehrE))
                {
                    ref var velocity = ref bulletA.Velocities[bulletE];

                    ref var hit = ref relA.HitRequests.Add(relE);
                    ref var immunity = ref relA.Immunities.Add(relE);
                    immunity.TimeLeft = 0.2f;
                    hit.directionNormal = velocity.lineral.normalized;
                }

            }
        }
    }
}
