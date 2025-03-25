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

        class EventAspect : EcsAspect
        {
            public readonly EcsTagPool<SpawnStarshipEvent> SpawnStarships = Inc;
        }
        class SpawnAspect : EcsAspect
        {
            public readonly EcsPool<PoolID> PoolIDs = Inc;
            public readonly EcsPool<TransformData> TransformDatas = Inc;
            public readonly EcsPool<Starship> Starships = Inc;
            public readonly EcsPool<InputData> InputDatas = Inc;
            public readonly EcsPool<Immunity> Immunities = Inc;
            public readonly EcsPool<WrapAroundScreenMarker> WrapAroundScreenMarkers = Inc;
            public readonly EcsPool<RequestIntersectionEvent> RequestIntersectionEvents = Inc;
        }

        public void Run()
        {
            var spawnA = _world.GetAspect<SpawnAspect>();
            foreach (var eventE in _world.Where(out EventAspect eventA))
            {
                var newE = _world.NewEntity(_staticData.PlayerStarshipTemplate);
                var newViewInstance = _poolService.Get(_staticData.StarshipViewPrefab, out spawnA.PoolIDs.TryAddOrGet(newE));
                newViewInstance.Connect((_world, newE), false);
                spawnA.Apply(_world, newE);

                spawnA.Starships[newE].View = newViewInstance;
                spawnA.Immunities[newE].TimeLeft = _staticData.StarshipSpawnImmunityTime;

                ref var newTransformData = ref spawnA.TransformDatas[newE];
                newTransformData.position = _sceneData.SpawnPosition.position;
                newTransformData.rotation = _sceneData.SpawnPosition.rotation;

                ref var newWantIntersectionWithAsteroid = ref spawnA.RequestIntersectionEvents[newE];
                newWantIntersectionWithAsteroid.CheckRadius = _staticData.AsteroidViewPrefab.Radius + newViewInstance.Radius;
                newWantIntersectionWithAsteroid.ObjectRadius = newViewInstance.Radius;
            
                eventA.SpawnStarships.Del(eventE);
            }
        }
    }
}