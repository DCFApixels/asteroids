using Asteroids.GameFieldFueature;
using DCFApixels.DragonECS;
using System.Linq;
using UnityEngine;

namespace Asteroids.BulletsFeature
{
    public class BulletLifeTimeSystem : IEcsRun
    {
        [DI] EcsDefaultWorld _world;
        class Aspect : EcsAspect
        {
            public EcsPool<Bullet> Bullets = Inc;
            public EcsTagPool<WrapAroundGameFieldMarker> WrapAroundGameFieldMarkers = Inc;
            public EcsTagPool<KillOutsideGameFieldMarker> KillOutsideGameFieldMarkers = Exc;
        }
        public void Run()
        {
            foreach (var e in _world.Where(out Aspect a))
            {
                ref var bullet = ref a.Bullets[e];
                if (bullet.lifeTime > 0)
                {
                    bullet.lifeTime -= Time.deltaTime;
                    if (bullet.lifeTime < 0)
                    {
                        a.WrapAroundGameFieldMarkers.Del(e);
                        a.KillOutsideGameFieldMarkers.Add(e);
                    }
                }

            }
        }
    }
}
