using Asteroids.BulletsFeature;
using Asteroids.GameFieldFueature;
using Asteroids.LocalInputFeature;
using Asteroids.StarshipInputControlFeature;
using Asteroids.StartshipsFeature;
using Asteroids.Systems;
using DCFApixels.DragonECS;
using Modules.BoundsOverlaps;
using Modules.CameraController;
using Modules.Movement;
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
        [SerializeField]
        private RuntimeData RuntimeData;

        private void Start()
        {
            _world = new EcsDefaultWorld();
            _graphWorld = new EcsGraphWorld();
            var graph = _world.CreateGraph(_graphWorld);

            _pipeline = EcsPipeline.New()
                .AddUnityDebug(_world, _graphWorld)
                // Adding systems.
                .Add(new InitSystem())
                .Add(new ChangeStateSystem())

                .Add(new SpawnStarshipSystem())
                .AddModule(new LocalInputModule())

                .AddModule(new AsteroidModule())

                .Add(new OutOfGameFieldBehaviorSystem())
                .Add(new PlayDeathVFXSystem())
                .Add(new UIUpdateSystem())
                .Add(new RestartSystem())

                .AddModule(new CameraControllerModule())
                .AddModule(new StarshipInputControlModule())
                .AddModule(new MovementModule())
                .AddModule(new BulletsModule())

                .AddModule(new GameFieldModule())
                .AddModule(new BoundsOverlapsModule())

                .AddModule(new StartshipsModule())
                .Add(new DeleteKilledEntitesSystem())

                // Injecting into systems.
                .Inject(_world)
                .Inject(_graphWorld)
                .Inject(graph)
                .Inject(StaticData)
                .Inject(SceneData)
                .Inject(RuntimeData)
                .AutoInject()

                .Add(new DebugEntitiesSystem())

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