using Asteroids.Components;
using Asteroids.Data;
using DCFApixels.DragonECS;
using UnityEngine;

namespace Asteroids.Systems
{
    internal class CheckAsteroidHitSystem : IEcsRun
    {
        [DI] private RuntimeData _runtimeData;
        [DI] private EcsDefaultWorld _world;

        class Aspect : EcsAspect
        {
            public readonly EcsPool<Asteroid> Asteroid = Inc;
            public readonly EcsPool<Hit> Hit = Inc;
            public readonly EcsPool<TransformRef> Transform = Inc;
        }
        public void Run()
        {
            foreach (var e in _world.Where(out Aspect a))
            {
                ref var asteroid = ref a.Asteroid.Get(e);
            
                if (asteroid.DeathsLeft > 0)
                {
                    asteroid.DeathsLeft--;

                    ref var asteroidTransform = ref a.Transform.Get(e).Value;

                    var forward = Vector3.forward;
                
                    var hitByObjectEntLong = a.Hit.Get(e).ByObject;
                    if (hitByObjectEntLong.TryUnpack(out var hitByEntity, out short _))
                    {
                        var hitByPosition = a.Transform.Get(hitByEntity).Value;
                        if (hitByPosition.position != asteroidTransform.position)
                        {
                            forward = asteroidTransform.position - hitByPosition.position;
                        }
                    }

                    var spawnPool = _world.GetPool<SpawnAsteroid>();
                
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