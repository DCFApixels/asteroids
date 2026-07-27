using DCFApixels.DragonECS;
using UnityEngine;

namespace Modules.FX
{
    [MetaGroup(FXModule.META_GROUP)]
    [MetaColor(FXModule.META_COLOR)]
    public class FXLifeTimeSystem : IEcsRun
    {
        [DI] EcsDefaultWorld _world;
        class Aspect : EcsAspect
        {
            public EcsPool<FXLifeTime> LifeTimes = Inc;
            public EcsTagPool<FXLifeTimeElapsedEvent> ElapsedEvents = Exc;
        }
        public void Run()
        {
            _world.GetAspects(out Aspect a);
            a.ElapsedEvents.ClearAll();
            foreach (var e in a.LifeTimes.UpdateTime(Time.deltaTime))
            {
                a.ElapsedEvents.Add(e);
            }
        }
    }
}
