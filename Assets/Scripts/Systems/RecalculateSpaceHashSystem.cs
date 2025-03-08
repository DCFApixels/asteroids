using Asteroids.Components;
using Asteroids.Data;
using DCFApixels.DragonECS;
using JetBrains.Annotations;

namespace Asteroids.Systems
{
    internal class RecalculateSpaceHashSystem : IEcsRun
    {
        [DI] private RuntimeData _runtimeData;
        [DI] private EcsDefaultWorld _world;

        private class AsteroidAspect : EcsAspect
        {
            public readonly EcsPool<MoveInfo> MoveInfos = Inc;
            [UsedImplicitly]
            public readonly EcsPool<Asteroid> Asteroids = Inc;
        }
        public void Run()
        {
            _runtimeData.AreaHash.Clear();

            foreach (var e in _world.Where(out AsteroidAspect a))
            {
                var position = a.MoveInfos.Get(e).Position;
                _runtimeData.AreaHash.Add(_world.GetEntityLong(e), position.x, position.z);
            }
        }
    }
}