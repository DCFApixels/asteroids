using Asteroids.AsteroidsFeature;
using Asteroids.BulletsFeature;
using Asteroids.GameFieldFeature;
using Asteroids.LocalInputFeature;
using Asteroids.StarshipInputControlFeature;
using Asteroids.StarshipsFeature;
using Asteroids.Systems;
using DCFApixels.DragonECS;
using Modules.BoundsOverlaps;
using Modules.CameraController;
using Modules.FX;
using Modules.Motion;
using UnityEngine;

namespace Asteroids
{
    public class Game : MonoBehaviour
    {
        private EcsPipeline _pipeline;
        private EcsDefaultWorld _world;
        private EcsGraphWorld _graphWorld;

        [SerializeField]
        private ConfigData StaticData;
        [SerializeField] 
        private SceneData SceneData;

        private void Start()
        {
            _world = new EcsDefaultWorld();
            _graphWorld = new EcsGraphWorld();
            var graph = _world.CreateGraph(_graphWorld);

            _pipeline = EcsPipeline.New()
                // Adding systems.
                .Add(new InitSystem())
                .Add(new ChangeStateSystem())

                .AddModule(new LocalInputModule())

                .AddModule(new AsteroidModule())

                .Add(new UIUpdateSystem())
                .Add(new RestartSystem())

                .AddModule(new CameraControllerModule())
                .AddModule(new StarshipInputControlModule())
                .AddModule(new MovementModule())
                .AddModule(new BulletsModule())
                .AddModule(new FXModule())
                .Add(new PlayDeathVFXSystem())

                .AddModule(new GameFieldModule())
                .AddModule(new BoundsOverlapsModule())

                .AddModule(new StarshipsModule())
                .Add(new DeleteKilledEntitesSystem())

                // Injecting into systems.
                .Inject(_world, _graphWorld, graph, StaticData, SceneData)
                .AddUnityDebug(_world, _graphWorld)
                .Add(new DebugEntitiesSystem())
                .AutoInject()
                .BuildAndInit();

        }

        private void FixedUpdate()
        {
            _pipeline.FixedRun();
        }
        private void Update()
        {
            _pipeline.Run();
        }
        private void OnDestroy()
        {
            _pipeline.Destroy();
            _pipeline = null;
            
            _world.Destroy();
            _world = null;
            
            _graphWorld.Destroy();
            _graphWorld = null;
        }
    }
}
