using Asteroids.Views;
using DCFApixels.DragonECS;
using UnityEngine;

namespace Asteroids.VFX
{
    [MetaGroup(VFXModule.META_GROUP)]
    [MetaColor(VFXModule.META_COLOR)]
    public class VFXLifeTimeSystem : IEcsRun
    {
        [DI] EcsDefaultWorld _world;
        class Aspect : EcsAspect
        {
            public EcsRefPool<VFXView> Views = Inc;
            public EcsPool<VFXLifeTime> LifeTimes = Inc;
            public EcsTagPool<VFXLifeTimeElapsedEvent> ElapsedEvents = Exc;
        }
        public void Run()
        {
            _world.GetAspects(out Aspect a);
            foreach (var e in a.LifeTimes.UpdateTime(Time.deltaTime))
            {
                a.ElapsedEvents.Add(e);
            }
        }
    }
}
