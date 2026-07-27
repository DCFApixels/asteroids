using Asteroids.Components;
using DCFApixels.DragonECS;

namespace Asteroids.StarshipsFeature
{
    [MetaGroup(StarshipsModule.META_GROUP, EcsConsts.SYSTEMS_GROUP)]
    [MetaColor(StarshipsModule.META_COLOR)]
    class RespawnStarshipOnDeathSystem : IEcsRun
    {
        [DI] EcsDefaultWorld _world;
        [DI] GameRuntimeData _gameRuntime;
        [DI] StarshipsRuntimeData _runtime;

        class StarshipAspect : EcsAspect
        {
            public EcsPool<Starship> Starships = Inc;
        }

        public void Run()
        {
            var starshipA = _world.GetAspect<StarshipAspect>();

            if (starshipA.Starships.Count != 0 || _gameRuntime.GameState != GameState.Play)
            {
                return;
            }

            _runtime.LifeLeft--;
            if (_runtime.LifeLeft == 0)
            {
                _world.GetPool<ChangeState>().Add(_world.NewEntity()).NextState = GameState.Lose;
                return;
            }

            _world.GetPool<SpawnStarshipRequest>().NewEntity();
        }
    }
}
