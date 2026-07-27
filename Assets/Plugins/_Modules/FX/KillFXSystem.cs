using DCFApixels.DragonECS;

namespace Modules.FX
{
    [MetaGroup(FXModule.META_GROUP)]
    [MetaColor(FXModule.META_COLOR)]
    public class KillFXSystem : IEcsRun
    {
        [DI] EcsDefaultWorld _world;
        class Aspect : EcsAspect
        {
            public EcsTagPool<FXLifeTimeElapsedEvent> ElapsedEvents = Inc;
        }
        public void Run()
        {
            foreach (var e in _world.Where(out Aspect a))
            {
                _world.DelEntity(e);
            }
        }
    }
}
