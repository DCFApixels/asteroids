using DCFApixels.DragonECS;
using UnityEngine;

namespace Asteroids.BulletsFeature
{
    public class BulletLifeTimeSystem : IEcsRun
    {
        [DI] EcsDefaultWorld _world;
        class Aspect : EcsAspect
        {
            public EcsPool<Projectile> Bullets = Inc;
            public EcsPool<ProjectileLifetime> BulletLifetimes = Inc;

            public EcsPool<OutOfGameFieldBehavior> OutOfGameFieldBehaviors = Opt;
        }
        public void Run()
        {
            _world.GetAspects(out Aspect a);
            foreach (var e in a.BulletLifetimes.UpdateTime(Time.deltaTime))
            {
                a.BulletLifetimes.Del(e);
                a.OutOfGameFieldBehaviors.TryAddOrGet(e).Mode = OutOfGameFieldBehaviorMode.KillImmediate;
            }
        }
    }
}
