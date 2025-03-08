using Asteroids.Components;
using DCFApixels.DragonECS;
using UnityEngine;

namespace Asteroids.Systems
{
    internal class CheckAsteroidHitSystem : IEcsRun
    {
        [DI] EcsDefaultWorld _world;

        class Aspect : EcsAspect
        {
            public readonly EcsPool<Asteroid> Asteroids = Inc;
            public readonly EcsPool<Hit> Hits = Inc;
            public readonly EcsPool<TransformRef> TransformRefs = Inc;
        }
        public void Run()
        {
            foreach (var e in _world.Where(out Aspect a))
            {
                ref var asteroid = ref a.Asteroids.Get(e);

                if (asteroid.DeathsLeft > 0)
                {
                    asteroid.DeathsLeft--;

                    ref var asteroidTransform = ref a.TransformRefs.Get(e).Value;

                    var forward = Vector3.forward;

                    var hitByObjectEntLong = a.Hits.Get(e).ByObject;
                    if (hitByObjectEntLong.TryUnpack(out var hitByEntity, out short _))
                    {
                        var hitByPosition = a.TransformRefs.Get(hitByEntity).Value;
                        if (hitByPosition.position != asteroidTransform.position)
                        {
                            forward = asteroidTransform.position - hitByPosition.position;
                        }
                    }

                    var spawnPool = _world.GetPool<SpawnAsteroidEvent>();

                    for (int i = 0; i < 2; i++)
                    {
                        var startForward = Quaternion.Euler(0, 90 + 180 * i, 0) * forward;

                        ref var spawnAsteroid = ref spawnPool.Add(_world.NewEntity());
                        spawnAsteroid.DeathsLeft = asteroid.DeathsLeft;
                        spawnAsteroid.Position = asteroidTransform.position;
                        spawnAsteroid.Rotation = Quaternion.LookRotation(startForward);
                        spawnAsteroid.StartRadius = asteroid.Radius / 2f;
                    }
                }
            }

        }
    }
}