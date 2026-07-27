using DCFApixels;
using DCFApixels.DragonECS;
using System;
using UnityEngine;

namespace Modules.Motion
{
    [MetaGroup(MovementModule.META_GROUP)]
    [MetaColor(MovementModule.META_COLOR)]
    internal class DebugVelocitySystem : IEcsRun, IEcsDefaultAddParams
    {
        public AddParams AddParams => EcsConsts.POST_END_LAYER;

        const string StarshipTypeName = "Asteroids.StarshipsFeature.Starship";

        [DI] EcsDefaultWorld _world;
        IEcsPool _starships;

        class VeloctityViewAspect : EcsAspect
        {
            public EcsPool<RigidTransform> RigidTransform = Inc;
            public EcsPool<Velocity> Velocities = Inc;
        }

        public void Run()
        {
            foreach (var e in _world.Where(out VeloctityViewAspect a))
            {
                ref var velocity = ref a.Velocities[e];
                ref var transform = ref a.RigidTransform[e];

                DebugX.Draw(new Color(1, 1, 1, 0.3f) * Color.cyan).RayArrow(transform.Position, velocity.Lineral / 2f);

                if (IsStarship(e))
                {
                    DebugX.Draw(Color.cyan).Text(
                        transform.Position,
                        $"Speed: {velocity.Lineral.magnitude:0.00}",
                        DebugXTextSettings.WorldSpace.Size(18).Anchor(TextAnchor.MiddleCenter));
                }
            }
        }

        bool IsStarship(int entity)
        {
            if (_starships == null && TryFindStarshipPool(out var starships))
            {
                _starships = starships;
            }

            return _starships != null && _starships.Has(entity);
        }

        bool TryFindStarshipPool(out IEcsPool starships)
        {
            var starshipType = Type.GetType($"{StarshipTypeName}, Assembly-CSharp");
            if (starshipType != null && _world.TryFindPoolInstance(starshipType, out starships))
            {
                return true;
            }

            foreach (var pool in _world.AllPools)
            {
                if (pool != null && pool.ComponentType.FullName == StarshipTypeName)
                {
                    starships = pool;
                    return true;
                }
            }

            starships = null;
            return false;
        }
    }
}
