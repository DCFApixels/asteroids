using Asteroids.Data;
using Asteroids.Utils;
using DCFApixels.DragonECS;

namespace Asteroids.Systems
{
    internal class InitSystem : IEcsInit
    {
        [DI] private EcsDefaultWorld _world;

        [DI] private StaticData _staticData;
        [DI] private PoolService _poolService;
        [DI] private SceneData _sceneData;
        [DI] private RuntimeData _runtimeData;
    
        public void Init()
        {
            _poolService.PreWarm(_staticData.AsteroidView, 20);
            _poolService.PreWarm(_staticData.BulletView, 20);
            _poolService.PreWarm(_staticData.StarshipView, 1);
            _poolService.PreWarm(_staticData.AsteroidExplosion, 20);
           
            _sceneData.UI.LoseScreen.InjectWorld(_world);

            _world.GetPool<ChangeState>().Add(_world.NewEntity()).NewState = GameState.Play;
        }
    }
}