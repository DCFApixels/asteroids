using Asteroids.Components;
using Asteroids.Data;
using Asteroids.Utils;
using DCFApixels.DragonECS;

namespace Asteroids.Systems
{
    internal class SpawnStarshipSystem : IEcsRun
    {
        [DI] EcsDefaultWorld _world;
        [DI] StaticData _staticData;
        [DI] SceneData _sceneData;
        [DI] PoolService _poolService;

        class EventAspect : EcsAspect
        {
            public readonly EcsTagPool<SpawnStarshipEvent> SpawnStarshipEvents = Inc;
        }
        class StarshipAspect : EcsAspect
        {
            public readonly EcsPool<PoolID> PoolIDs = Inc;
            public readonly EcsPool<Starship> Starships = Inc;
            public readonly EcsPool<TransformRef> TransformRefs = Inc;
            public readonly EcsPool<MoveInfo> MoveInfos = Inc;
            public readonly EcsPool<InputData> InputDatas = Inc;
            public readonly EcsPool<Cycle> Cycles = Inc;
            public readonly EcsPool<RequestIntersection> RequestIntersections = Inc;
        }

        public void Run()
        {
            var startshipA = _world.GetAspect<StarshipAspect>();
            foreach (var eventE in _world.Where(out EventAspect eventA))
            {
                var prefab = _staticData.StarshipView;
                var instance = _poolService.Get(prefab, out var id);
                instance.transform.position = _sceneData.SpawnPosition.position;
                instance.transform.rotation = _sceneData.SpawnPosition.rotation;

                var startshipE = _world.NewEntity(startshipA);
                ref var poolId = ref startshipA.PoolIDs.Get(startshipE);
                ref var starship = ref startshipA.Starships.Get(startshipE);
                ref var transformRef = ref startshipA.TransformRefs.Get(startshipE);
                ref var moveInfo = ref startshipA.MoveInfos.Get(startshipE);
                ref var inputData = ref startshipA.InputDatas.Get(startshipE);
                ref var cycle = ref startshipA.Cycles.Get(startshipE);
                ref var wantIntersectionWithAsteroid = ref startshipA.RequestIntersections.Get(startshipE);

                poolId.Id = id;
                poolId.Component = instance;

                starship.View = instance;
                transformRef.Value = instance.transform;

                moveInfo.DefaultRotationSpeed = _staticData.RotationSpeed;
                moveInfo.DefaultSpeed = _staticData.StarshipSpeed;
                moveInfo.Position = instance.transform.position;
                moveInfo.Forward = instance.transform.forward;

                wantIntersectionWithAsteroid.CheckRadius = _staticData.AsteroidView.Radius + instance.Radius;
                wantIntersectionWithAsteroid.ObjectRadius = instance.Radius;

                eventA.SpawnStarshipEvents.Del(eventE);
            }
        }
    }
}