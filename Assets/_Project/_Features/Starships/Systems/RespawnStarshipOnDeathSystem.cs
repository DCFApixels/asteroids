using Asteroids.Components;
using DCFApixels.DragonECS;

namespace Asteroids.StarshipsFeature
{
    internal class RespawnStarshipOnDeathSystem : IEcsRun
    {
        [DI] private EcsDefaultWorld _world;
        [DI] private RuntimeData r;

        class StarshipAspect : EcsAspect
        {
            public EcsPool<Starship> Starships = Inc;
        }

        public void Run()
        {
            var starshipA = _world.GetAspect<StarshipAspect>();

            if (starshipA.Starships.Count != 0 || r.GameState != GameState.Play)
            {
                return;
            }

            r.LifeLeft--;
            if (r.LifeLeft == 0)
            {
                _world.GetPool<ChangeState>().Add(_world.NewEntity()).NextState = GameState.Lose;
                return;
            }

            _world.GetPool<SpawnStarshipRequest>().NewEntity();
        }
    }
}
