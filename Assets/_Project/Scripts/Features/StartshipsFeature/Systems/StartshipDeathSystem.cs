using Asteroids.Components;
using Asteroids.Data;
using Asteroids.MovementFeature;
using Asteroids.Utils;
using DCFApixels.DragonECS;

namespace Asteroids.StartshipsFeature
{
    internal class StartshipDeathSystem : IEcsRun
    {
        [DI] EcsDefaultWorld _world;
        [DI] RuntimeData _runtimeData;
        [DI] SceneData _sceneData;
        [DI] StaticData _staticData;
        [DI] PoolService _poolService;

        class Aspect : EcsAspect
        {
            public EcsPool<Starship> Starships = Inc;
            public EcsPool<TransformData> TransformDatas = Inc;
            public EcsPool<HitSignal> HitSignals = Inc;
        }
        public void Run()
        {
            foreach (var e in _world.Where(out Aspect a))
            {
                ref var transformData = ref a.TransformDatas[e];

                var explosion = _poolService.Get(_staticData.StarshipExplosionPrefab, out var instanceID);
                explosion.transform.position = transformData.position;
                explosion.Play(_poolService, instanceID);
            }
        }
    }
}
