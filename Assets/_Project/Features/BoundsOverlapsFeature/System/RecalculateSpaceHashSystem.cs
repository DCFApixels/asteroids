using Asteroids.Data;
using Asteroids.MovementFeature;
using DCFApixels.DragonECS;

namespace Asteroids.Systems
{
    internal class RecalculateSpaceHashSystem : IEcsRun
    {
        [DI] private RuntimeData _runtimeData;
        [DI] private EcsDefaultWorld _world;

        public void Run()
        {
            _runtimeData.AreaHash.Clear();

            //foreach (var e in _world.Where(out  CombinedAspect<SingleAspect<TransformData>, SingleAspect<Asteroid>>  a))
            //{
            //    var position = a.a0.pool[e].position;
            //    _runtimeData.AreaHash.Add((e, _world), position.x, position.z);
            //}

            foreach (var e in _world.Where(out SingleAspect<TransformData>  a))
            {
                var position = a.pool[e].position;
                _runtimeData.AreaHash.Add((e, _world), position.x, position.z);
            }

        }
    }
}