using Asteroids.Components;
using DCFApixels.DragonECS;
using Modules.BoundsOverlaps;
using Modules.Motion;
using UnityEngine;

namespace Asteroids.AsteroidsFeature
{
    internal class SpawnAsteroidSystem : IEcsRun
    {
        [DI] AsteroidsFeatureConfig c;
        [DI] EcsDefaultWorld _world;

        class EventAspect : EcsAspect
        {
            public EcsPool<SpawnAsteroidRequest> Requests = Inc;
        }
        class SpawnAspect : EcsAspect
        {
            public EcsPool<Asteroid> Asteroids = Inc;
            public EcsPool<BoundsSphere> BoundsSpheres = Inc;
            public EcsPool<Velocity> Velocities = Inc;
            public EcsPool<RigidTransform> RigidTransforms = Inc;
        }

        public void Run()
        {
            _world.GetAspects(out SpawnAspect spawnA, out EventAspect eventA);
            foreach (var newE in _world.Where(eventA))
            {
                var req = eventA.Requests.Get(newE);

                req.Description.Apply(_world, newE);
                spawnA.Apply(_world, newE);

                ref var newAsteroid = ref spawnA.Asteroids.TryAddOrGet(newE);
                newAsteroid.Description = req.Description;
                newAsteroid.DeathsLeft = req.OverrideDeathsCount;

                ref var newBounds = ref spawnA.BoundsSpheres[newE];
                newBounds.Radius = req.OverrideRadius;
                newAsteroid.View.SetRadius(newBounds.Radius); 

                ref var newTransformData = ref spawnA.RigidTransforms.TryAddOrGet(newE);
                newTransformData.Position = req.Position;
                newTransformData.Rotation = req.Rotation;

                ref var newVelocity = ref spawnA.Velocities.TryAddOrGet(newE);
                newVelocity.Lineral = newTransformData.ToLocalVector(Vector3.forward) * Random.Range(c.AsteroidMinSpeed, c.AsteroidMaxSpeed);
            }
            eventA.Requests.ClearAll();
        }

    }
}
