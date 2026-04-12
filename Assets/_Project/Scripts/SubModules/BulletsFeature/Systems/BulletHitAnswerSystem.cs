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
            public EcsPool<Bullet> Bullets = Inc;
            public EcsPool<KillRequest> KillRequests = Exc;
        }

        public void Run()
        {
            _graph.GraphWorld.GetAspects(out RelAspect relA);
            var bulletEs = _graph.World.WhereToGroup(out BulletAspect bulletA);

            foreach (var relE in _graph.GraphWorld.Where(relA))
            {
                ref var answ = ref relA.HitAnswers[relE];
                var bulletE = _graph.GetRelationStart(relE);
                if (bulletEs.Has(bulletE))
                {
                    ref var killReq = ref bulletA.KillRequests.TryAddOrGet(bulletE);
                    killReq.Normal = answ.CollisionNormal;
                }
            }

            relA.HitAnswers.ClearAll();
        }
    }
}
