using Asteroids.GameFieldFeature;
using Asteroids.Components;
using Asteroids.StarshipsFeature;
using DCFApixels.DragonECS;
using Modules.Motion;

namespace Asteroids.StarshipsFeature
{
    [MetaGroup(StarshipsModule.META_GROUP, EcsConsts.SYSTEMS_GROUP)]
    [MetaColor(StarshipsModule.META_COLOR)]
    class SpawnStarshipSystem : IEcsRun
    {
        [DI] EcsDefaultWorld _world;
        [DI] StarshipsFeatureConfig _config;

        class RequestAspect : EcsAspect
        {
            public readonly EcsPool<SpawnStarshipRequest> SpawnStarshipRequests = Inc;
        }
        class SpawnAspect : EcsAspect
        {
            public readonly EcsPool<RigidTransform> RigidTransforms = Inc;
            public readonly EcsPool<Starship> Starships = Inc;
            public readonly EcsPool<HitImmunity> Immunities = Inc;
            public readonly EcsPool<OutOfGameFieldBehavior> OutOfGameFieldBehaviors = Inc;
        }

        public void Run()
        {
            _world.GetAspects(out SpawnAspect spawnA, out RequestAspect eventA);
            foreach (var eventE in _world.Where(eventA))
            {
                ref var req = ref eventA.SpawnStarshipRequests[eventE];

                var newE = _world.NewEntity(_config.PlayerStarshipTemplate);
                spawnA.Apply(_world, newE);

                spawnA.Immunities[newE].TimeLeft = _config.StarshipSpawnImmunityTime;

                ref var newRigidTransform = ref spawnA.RigidTransforms[newE];
                newRigidTransform.Position = req.Position;
                newRigidTransform.Rotation = req.Rotation;

            }

            eventA.SpawnStarshipRequests.ClearAll();
        }
    }
}
