using Asteroids.Components;
using Asteroids.StartshipsFeature;
using DCFApixels.DragonECS;
using Modules.Movement;

namespace Asteroids.Systems
{
    internal class SpawnStarshipSystem : IEcsRun
    {
        [DI] EcsDefaultWorld _world;
        [DI] ConfigData c;

        class RequestAspect : EcsAspect
        {
            public readonly EcsPool<SpawnStarshipRequest> Requests = Inc;
        }
        class SpawnAspect : EcsAspect
        {
            public readonly EcsPool<RigidTransform> TransformDatas = Inc;
            public readonly EcsPool<Starship> Starships = Inc;
            public readonly EcsPool<HitImmunity> Immunities = Inc;
            public readonly EcsPool<OutOfGameFieldBehavior> OutOfGameFieldBehaviors = Inc;
        }

        public void Run()
        {
            _world.GetAspects(out SpawnAspect spawnA, out RequestAspect eventA);
            foreach (var eventE in _world.Where(eventA))
            {
                ref var req = ref eventA.Requests[eventE];

                var newE = _world.NewEntity(c.PlayerStarshipTemplate);
                spawnA.Apply(_world, newE);

                spawnA.Immunities[newE].TimeLeft = c.StarshipSpawnImmunityTime;

                ref var newTransformData = ref spawnA.TransformDatas[newE];
                newTransformData.Position = req.Position;
                newTransformData.Rotation = req.Rotation;

            }

            eventA.Requests.ClearAll();
        }
    }
}