using Asteroids.Components;
using Asteroids.Data;
using Asteroids.MovementFeature;
using Asteroids.Utils;
using DCFApixels.DragonECS;

namespace Asteroids.Systems
{
    internal class SpawnStarshipSystem : IEcsRun
    {
        [DI] private EcsDefaultWorld _world;
        [DI] private StaticData _staticData;
        [DI] private SceneData _sceneData;
        [DI] private PoolService _poolService;

        private class EventAspect : EcsAspect
        {
            public readonly EcsTagPool<SpawnStarshipEvent> SpawnStarships = Inc;
        }
    
        public void Run()
        {
            foreach (var eventE in _world.Where(out EventAspect eventA))
            {
                var startshipInstance = _poolService.Get(_staticData.StarshipView, out var startshipInstanceID);
                
                var startshipE = _world.NewEntity(_staticData.PlayerStarshipTemplate);

                ref var poolId = ref _world.GetPool<PoolId>().TryAddOrGet(startshipE);
                poolId.Id = startshipInstanceID;
                poolId.Component = startshipInstance;
            
                _world.GetPool<Starship>().TryAddOrGet(startshipE).View = startshipInstance;
                startshipInstance.Connect((_world, startshipE), false);
                //_world.GetPool<UnityComponent<Transform>>().Add(entity).obj = instance.transform;
                _world.GetPool<Immunity>().TryAddOrGet(startshipE).TimeLeft = _staticData.StarshipSpawnImmunityTime;

                ref var transformData = ref _world.GetPool<TransformData>().TryAddOrGet(startshipE);
                transformData.position = _sceneData.SpawnPosition.position;
                transformData.rotation = _sceneData.SpawnPosition.rotation;
                //ref var moveInfo = ref _world.GetPool<MoveInfo>().Add(entity);
                //moveInfo.DefaultRotationSpeed = _staticData.RotationSpeed;
                //moveInfo.Speed = _staticData.StarshipSpeed;
                //moveInfo.Position = instance.transform.position;
                //moveInfo.Forward = instance.transform.forward;
                //moveInfo.MaxSpeed = _staticData.StarshipMaxSpeed;
                //moveInfo.Acceleration = _staticData.StarshipAcceleration;
                //moveInfo.Friction = _staticData.StarshipFriction;

                _world.GetPool<InputData>().TryAddOrGet(startshipE);
                _world.GetPool<WrapAroundScreenMarker>().TryAddOrGet(startshipE);
                ref var wantIntersectionWithAsteroid = ref _world.GetPool<RequestIntersectionEvent>().TryAddOrGet(startshipE);
                wantIntersectionWithAsteroid.CheckRadius = _staticData.AsteroidView.Radius + startshipInstance.Radius;
                wantIntersectionWithAsteroid.ObjectRadius = startshipInstance.Radius;
            
                eventA.SpawnStarships.Del(eventE);
            }
        }
    }
}