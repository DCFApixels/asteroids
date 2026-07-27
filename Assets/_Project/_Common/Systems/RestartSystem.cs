using Asteroids.Components;
using DCFApixels.DragonECS;

namespace Asteroids.Systems
{
    internal class RestartSystem : IEcsRun
    {
        [DI] EcsDefaultWorld _world;

        class Aspect : EcsAspect
        {
            public EcsTagPool<RestartEvent> RestartEvents = Inc;
        }
        public void Run()
        {
            foreach (var _ in _world.Where(out Aspect _))
            {
                foreach (var entity in _world.Entities)
                {
                    _world.DelEntity(entity);
                }

                _world.GetPool<ChangeState>().Add(_world.NewEntity()).NextState = GameState.Play;
            }
        }
    }
}
