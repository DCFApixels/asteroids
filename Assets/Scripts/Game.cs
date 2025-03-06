using Asteroids.Data;
using Asteroids.Systems;
using Asteroids.Utils;
using DCFApixels.DragonECS;
using UnityEngine;

namespace Asteroids
{
    public class Game : MonoBehaviour
    {
        private EcsPipeline _pipeline;
        private EcsDefaultWorld _world;
        private EcsEventWorld _eventWorld;

        [SerializeField]
        private StaticData _staticData;

        [SerializeField] private SceneData _sceneData;
    
        [SerializeField]
        private RuntimeData _runtimeData;
        private void Start()
        {
            // Creating world for entities and components.
            _world = new EcsDefaultWorld();
            _eventWorld = new EcsEventWorld();
            // Creating pipeline for systems.
            _pipeline = EcsPipeline.New()
                .AddUnityDebug(_world, _eventWorld)
                // Adding systems.
                .Add(new InitSystem())
            
                .Add(new UpdateFieldSizeSystem())
            
                .AddModule(new StarshipModule())
                .AddModule(new AsteroidModule())
            
                .Add(new MoveSystem())
                .Add(new KillHitObjectSystem())
                .Add(new KillOutsideSystem())
                .Add(new CycleAroundScreenSystem())
            
                // Injecting world into systems.
                .Inject(_world)
                .Inject(_staticData)
                .Inject(_sceneData)
                .Inject(_runtimeData)
                .Inject(new PoolService())
                .AutoInject()
                // Other injections.
                // .Inject(SomeData)

                // Finalizing the pipeline construction.
                .Build();
            // Initialize the Pipeline and run IEcsPreInit.PreInit()
            // and IEcsInit.Init() on all added systems.
            _pipeline.Init();
        }
        private void Update()
        {
            // Invoking IEcsRun.Run() on all added systems.
            _pipeline.Run();
        }
        private void OnDestroy()
        {
            // Invoking IEcsDestroy.Destroy() on all added systems.
            _pipeline.Destroy();
            _pipeline = null;
            // Requires deleting worlds that will no longer be used.
            _world.Destroy();
            _world = null;
        }
    }
}