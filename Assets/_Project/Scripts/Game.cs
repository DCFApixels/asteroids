using Asteroids.BulletsFeature;
using Asteroids.CameraSmoothFollowFeature;
using Asteroids.ControlsFeature;
using Asteroids.Data;
using Asteroids.GameFieldFueature;
using Asteroids.MovementFeature;
using Asteroids.StarshipMovmentFeature;
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
        private StaticData StaticData;

        [SerializeField] private SceneData SceneData;
    
        [SerializeField]
        private RuntimeData RuntimeData;
        private void Start()
        {
            _world = new EcsDefaultWorld();
            _eventWorld = new EcsEventWorld();

            _pipeline = EcsPipeline.New()
                .AddUnityDebug(_world, _eventWorld)
                // Adding systems.
                .Add(new InitSystem())
                .Add(new ChangeStateSystem())
                .Add(new UpdateFieldSizeSystem())

                .AddModule(new StarshipModule())
                .AddModule(new AsteroidModule())

                .Add(new KillHitObjectSystem())
                .Add(new UIUpdateSystem())
                .Add(new RestartSystem())

                .AddModule(new CameraSmoothFollowModule())
                .AddModule(new ControlsModule())
                .AddModule(new StarshipMovmentModule())
                .AddModule(new MovementModule())
                .AddModule(new BulletsModule())
                .AddModule(new GameFieldModule())

                // Injecting into systems.
                .Inject(_world)
                .Inject(StaticData)
                .Inject(SceneData)
                .Inject(RuntimeData)
                .Inject(new PoolService())
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
            
            _eventWorld.Destroy();
            _eventWorld = null;
        }
    }
}