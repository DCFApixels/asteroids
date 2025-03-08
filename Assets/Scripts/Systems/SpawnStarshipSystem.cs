using Asteroids.Components;
using Asteroids.Data;
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

        private class Aspect : EcsAspect
        {
            public readonly EcsTagPool<SpawnStarshipEvent> SpawnStarships = Inc;
        }
    
        public void Run()
        {
            foreach (var e in _world.Where(out Aspect a))
            {
                var instance = _poolService.Get(_staticData.StarshipView, out var id);
                instance.transform.position = _sceneData.SpawnPosition.position;
                instance.transform.rotation = _sceneData.SpawnPosition.rotation;
                
                var entity = _world.NewEntity();
            
                ref var poolId = ref _world.GetPool<PoolId>().Add(entity);
                poolId.Id = id;
                poolId.Component = instance;
            
                _world.GetPool<Starship>().Add(entity).View = instance;
                _world.GetPool<TransformRef>().Add(entity).Value = instance.transform;
                _world.GetPool<Immunity>().Add(entity).TimeLeft = _staticData.StarshipSpawnImmunityTime;
            
                ref var moveInfo = ref _world.GetPool<MoveInfo>().Add(entity);
                moveInfo.DefaultRotationSpeed = _staticData.RotationSpeed;
                moveInfo.Speed = _staticData.StarshipSpeed;
                moveInfo.Position = instance.transform.position;
                moveInfo.Forward = instance.transform.forward;
                moveInfo.MaxSpeed = _staticData.StarshipMaxSpeed;
                moveInfo.Acceleration = _staticData.StarshipAcceleration;
                moveInfo.Friction = _staticData.StarshipFriction;

                _world.GetPool<InputData>().Add(entity);
                _world.GetPool<WrapAroundScreenMarker>().Add(entity);
                ref var wantIntersectionWithAsteroid = ref _world.GetPool<RequestIntersectionEvent>().Add(entity);
                wantIntersectionWithAsteroid.CheckRadius = _staticData.AsteroidView.Radius + instance.Radius;
                wantIntersectionWithAsteroid.ObjectRadius = instance.Radius;
            
                a.SpawnStarships.Del(e);
            }
        }
    }
}