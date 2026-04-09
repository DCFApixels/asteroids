using Asteroids.StarshipInputControlFeature;
using Asteroids.Views;
using DCFApixels.DragonECS;

namespace Asteroids.VFX
{
    [MetaGroup(VFXModule.META_GROUP)]
    [MetaColor(VFXModule.META_COLOR)]
    public class KillVFXSystem : IEcsRun
    {
        [DI] EcsDefaultWorld _world;
        class Aspect : EcsAspect
        {
            public EcsRefPool<VFXView> Views = Inc;
            public EcsTagPool<VFXLifeTimeElapsedEvent> ElapsedEvents = Inc;
        }
        public void Run()
        {
            foreach (var e in _world.Where(out Aspect a))
            {
                a.Views[e].Despawn();
                _world.DelEntity(e);
            }
        }
    }
}
