using Asteroids.Components;
using Asteroids.Data;
using Asteroids.Utils;
using DCFApixels.DragonECS;

namespace Asteroids.Systems
{
    internal class InitSystem : IEcsInit, IEcsDefaultAddParams
    {
        public AddParams AddParams => EcsConsts.PRE_BEGIN_LAYER;

        [DI] private EcsDefaultWorld _world;

        [DI] private StaticData _staticData;
        [DI] private PoolService _poolService;
        [DI] private SceneData _sceneData;
        [DI] private RuntimeData _runtimeData;

        public void Init()
        {
            _poolService.PreWarm(_staticData.AsteroidViewPrefab, 20);
            _poolService.PreWarm(_staticData.BulletViewPrefab, 20);
            _poolService.PreWarm(_staticData.StarshipViewPrefab, 1);
            _poolService.PreWarm(_staticData.AsteroidExplosionPrefab, 20);
            _poolService.PreWarm(_staticData.StarshipExplosionPrefab, 2);

            _sceneData.UI.LoseScreen.InjectWorld(_world);

            _world.GetPool<ChangeState>().Add(_world.NewEntity()).NextState = GameState.Play;
        }
    }
}