using Asteroids.Components;
using DCFApixels.DragonECS;

namespace Asteroids.Systems
{
    internal class InitSystem : IEcsInit, IEcsDefaultAddParams
    {
        public AddParams AddParams => EcsConsts.PRE_BEGIN_LAYER;

        [DI] EcsDefaultWorld _world;
        [DI] ConfigData c;
        [DI] SceneData s;
        [DI] RuntimeData r;

        public void Init()
        {
            s.UI.LoseScreen.InjectWorld(_world);

            _world.GetPool<ChangeState>().Add(_world.NewEntity()).NextState = GameState.Play;
        }
    }
}