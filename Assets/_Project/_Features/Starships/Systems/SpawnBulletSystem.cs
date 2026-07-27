using Asteroids.LocalInputFeature;
using DCFApixels.DragonECS;
using Modules.FX;
using Modules.Motion;
using System;
using UnityEngine;

namespace Asteroids.StarshipsFeature
{
    internal class SpawnBulletSystem : IEcsRun
    {
        [DI] EcsDefaultWorld _world;
        [DI] ConfigData c;

        class StarshipAspect : EcsAspect
        {
            public EcsPool<Starship> Starships = Inc;
            public EcsPool<FireInputBeginEvent> FireInputBeginSignals = Inc;
            public EcsPool<RigidTransform> TransformDatas = Inc;
            public EcsPool<Velocity> Velocities = Inc;
        }
        class SpawnAspect : EcsAspect
        {
            public EcsPool<RigidTransform> TransformDatas = Inc;
            public EcsPool<Velocity> Velocities = Inc;
        }

        public void Run()
        {
            var spawnA = _world.GetAspect<SpawnAspect>();
            foreach (var starshipE in _world.Where(out StarshipAspect starshipA))
            {
                var starshipTransformData = starshipA.TransformDatas.Get(starshipE);

                var newE = _world.NewEntity(c.ProjectileDescription);
                spawnA.Apply(_world, newE);

                ref var newTransformData = ref spawnA.TransformDatas[newE];
                newTransformData.Position = starshipTransformData.Position;
                newTransformData.Rotation = starshipTransformData.Rotation;

                ref var newVelocity = ref spawnA.Velocities[newE];
                newVelocity.Lineral = newTransformData.ToLocalVector(Vector3.forward) * (c.BulletSpeed + Math.Abs(starshipA.Velocities[starshipE].Lineral.magnitude));

                SpawnShootVFX(newTransformData);
            }
        }

        private void SpawnShootVFX(RigidTransform shotTransform)
        {
            if (c.ShootVFX == null)
            {
                return;
            }

            Vector3 forward = shotTransform.ToLocalVector(Vector3.forward);
            ref var request = ref _world.GetPool<ShortVFXSpawnRequest>().NewEntity();
            request.Prefab = c.ShootVFX;
            request.Position = shotTransform.Position + forward * c.ShootVFXForwardOffset;
            request.Rotation = shotTransform.Rotation;
            request.Direction = forward;
        }
    }
}
