using Asteroids.Components;
using DCFApixels.DragonECS;
using System.Linq;

namespace Asteroids.BulletsFeature
{
    class BulletHitAnswerSystem : IEcsRun
    {
        [DI] EntityGraph _graph;

        class RelAspect : EcsAspect
        {
            public EcsPool<HitAnswer> HitAnswers = Inc;
        }
        class BulletAspect : EcsAspect
        {
            public EcsPool<Projectile> Bullets = Inc;
            public EcsPool<KillRequest> KillSignals = Exc;
        }

        public void Run()
        {
            _graph.GraphWorld.GetAspects(out RelAspect relA);
            var bulletEs = _graph.World.WhereToGroup(out BulletAspect bulletA);

            foreach (var relE in _graph.GraphWorld.Where(relA))
            {
                var bulletE = _graph.GetRelationStart(relE);
                if (bulletEs.Has(bulletE))
                {
                    bulletA.KillSignals.TryAddOrGet(bulletE);
                }
            }

            relA.HitAnswers.ClearAll();
        }
    }
}
