using Asteroids.GameFieldFeature;
using DCFApixels.DragonECS;
using UnityEngine;

namespace Asteroids.BulletsFeature
{
    [MetaGroup(BulletsModule.META_GROUP, EcsConsts.SYSTEMS_GROUP)]
    [MetaColor(BulletsModule.META_COLOR)]
    class BulletLifetimeSystem : IEcsRun
    {
        [DI] EcsDefaultWorld _world;
        class Aspect : EcsAspect
        {
            public EcsPool<Bullet> Bullets = Inc;
            public EcsPool<BulletLifetime> BulletLifetimes = Inc;

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
