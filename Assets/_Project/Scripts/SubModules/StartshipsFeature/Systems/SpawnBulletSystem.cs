using Asteroids.LocalInputFeature;
using DCFApixels.DragonECS;
using Modules.Movement;
using System;
using UnityEngine;

namespace Asteroids.StartshipsFeature
{
    internal class SpawnBulletSystem : IEcsRun
    {
        [DI] EcsDefaultWorld _world;
        [DI] ConfigData c;

        class StashipAspect : EcsAspect
        {
            public EcsPool<Starship> Starships = Inc;
            public EcsPool<FireInputBeginEvent> FireInputBeginSignals = Inc;
            public EcsPool<RigidTransform> TransformDatas = Inc;
            public EcsPool<Velocity> Velocities = Inc;
        }
        class SpawnAspect : EcsAspect
        {
            public EcsPool<Velocity> Velocities = Inc;
            public EcsPool<RigidTransform> TransformDatas = Inc;
        }

        public void Run()
        {
            var spawnA = _world.GetAspect<SpawnAspect>();
            foreach (var stashipE in _world.Where(out StashipAspect stashipA))
            {
                var stashipTransformData = stashipA.TransformDatas.Get(stashipE);

                var newE = _world.NewEntity(c.ProjectileDescription);
                spawnA.Apply(_world, newE);

                ref var newTransformData = ref spawnA.TransformDatas[newE];
                newTransformData.Position = stashipTransformData.Position;
                newTransformData.Rotation = stashipTransformData.Rotation;

                ref var newVelocity = ref spawnA.Velocities[newE];
                newVelocity.Lineral = newTransformData.ToLocalVector(Vector3.forward) * (c.BulletSpeed + Math.Abs(stashipA.Velocities[stashipE].Lineral.magnitude));
            }
        }
    }
}