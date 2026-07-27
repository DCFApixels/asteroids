using Asteroids.LocalInputFeature;
using Asteroids.BulletsFeature;
using DCFApixels.DragonECS;
using Modules.FX;
using Modules.Motion;
using System;
using UnityEngine;

namespace Asteroids.StarshipsFeature
{
    [MetaGroup(StarshipsModule.META_GROUP, EcsConsts.SYSTEMS_GROUP)]
    [MetaColor(StarshipsModule.META_COLOR)]
    class SpawnBulletSystem : IEcsRun
    {
        [DI] EcsDefaultWorld _world;
        [DI] BulletsFeatureConfig _config;

        class StarshipAspect : EcsAspect
        {
            public EcsPool<Starship> Starships = Inc;
            public EcsPool<FireInputBeginEvent> FireInputBeginEvents = Inc;
            public EcsPool<RigidTransform> RigidTransforms = Inc;
            public EcsPool<Velocity> Velocities = Inc;
        }
        class SpawnAspect : EcsAspect
        {
            public EcsPool<RigidTransform> RigidTransforms = Inc;
            public EcsPool<Velocity> Velocities = Inc;
        }

        public void Run()
        {
            var spawnA = _world.GetAspect<SpawnAspect>();
            foreach (var starshipE in _world.Where(out StarshipAspect starshipA))
            {
                var starshipTransform = starshipA.RigidTransforms.Get(starshipE);

                var newE = _world.NewEntity(_config.ProjectileDescription);
                spawnA.Apply(_world, newE);

                ref var newRigidTransform = ref spawnA.RigidTransforms[newE];
                newRigidTransform.Position = starshipTransform.Position;
                newRigidTransform.Rotation = starshipTransform.Rotation;

                ref var newVelocity = ref spawnA.Velocities[newE];
                newVelocity.Lineral = newRigidTransform.ToLocalVector(Vector3.forward) * (_config.BulletSpeed + Math.Abs(starshipA.Velocities[starshipE].Lineral.magnitude));

                SpawnShootVFX(newRigidTransform);
            }
        }

        void SpawnShootVFX(RigidTransform shotTransform)
        {
            if (_config.ShootVFX == null)
            {
                return;
            }

            Vector3 forward = shotTransform.ToLocalVector(Vector3.forward);
            ref var request = ref _world.GetPool<ShortVFXSpawnRequest>().NewEntity();
            request.Prefab = _config.ShootVFX;
            request.Position = shotTransform.Position + forward * _config.ShootVFXForwardOffset;
            request.Rotation = shotTransform.Rotation;
            request.Direction = forward;
        }
    }
}
