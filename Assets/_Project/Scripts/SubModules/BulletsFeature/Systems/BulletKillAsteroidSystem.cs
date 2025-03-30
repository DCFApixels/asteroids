using Asteroids.BoundsOverlapsFeature;
using Asteroids.Components;
using Asteroids.MovementFeature;
using DCFApixels.DragonECS;
using System.Linq;

namespace Asteroids.BulletsFeature
{
    class BulletKillAsteroidSystem : IEcsRun
    {
        [DI] EntityGraph _graph;

        class RelAspect : EcsAspect
        {
            public EcsPool<OverlapsEvent> OverlapsEvents = Inc;
            public EcsTagPool<HitEvent> KillEvents = Exc;
        }
        class BulletAspect : EcsAspect
        {
            public EcsPool<Velocity> Velocities = Inc;
            public EcsPool<Bullet> Bullets = Inc;
        }
        class AsteroidAspect : EcsAspect
        {
            public EcsPool<Asteroid> Asteroids = Inc;
        }

        public void Run()
        {
            var bulletEs = _graph.World.WhereToGroup(out BulletAspect bulletA);
            var asteroidEs = _graph.World.WhereToGroup(out AsteroidAspect asteroidA);

            foreach (var e in _graph.GraphWorld.Where(out RelAspect a))
            {
                var (bulletE, asteroidE) = _graph.GetRelationStartEnd(e);
                if (bulletEs.Has(bulletE) && 
                    asteroidEs.Has(asteroidE))
                {
                    var relE = _graph.GetOrNewRelation(bulletE, asteroidE);
                    a.KillEvents.Add(relE);
                }
            }
        }
    }
}
