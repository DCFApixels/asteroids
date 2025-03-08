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
            _world = new();
            _eventWorld = new();
            
            _pipeline = EcsPipeline.New()
                .AddUnityDebug(_world, _eventWorld)
                // Adding systems.
                .Add(new InitSystem())
                .Add(new ChangeStateSystem())
                .Add(new UpdateFieldSizeSystem())
            
                .AddModule(new StarshipModule())
                .AddModule(new AsteroidModule())
            
                .Add(new MoveSystem())
                .Add(new KillHitObjectSystem())
                .Add(new WrapAroundScreenSystem())
                .Add(new KillOutsideSystem())
                .Add(new UIUpdateSystem())
                .Add(new RestartSystem())
            
                // Injecting into systems.
                .Inject(_world)
                .Inject(_staticData)
                .Inject(_sceneData)
                .Inject(_runtimeData)
                .Inject(new PoolService())
                .AutoInject()
               
                .BuildAndInit();
           
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
            
            _eventWorld.Destroy();
            _eventWorld = null;
        }
    }
}