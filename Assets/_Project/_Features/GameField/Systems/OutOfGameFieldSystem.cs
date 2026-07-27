using Asteroids.Components;
using DCFApixels.DragonECS;
using Modules.BoundsOverlaps;
using Modules.Motion;
using UnityEngine;

namespace Asteroids.GameFieldFeature
{
    internal class OutOfGameFieldSystem : IEcsRun
    {
        [DI] EcsDefaultWorld _world;
        [DI] GameFieldRuntimeData r;
        [DI] GameFieldModuleConfig c;

        private class Aspect : EcsAspect
        {
            public EcsPool<RigidTransform> RigidTransforms = Inc;
            public EcsPool<BoundsSphere> BoundsSpheres = Inc;
            public EcsPool<OutOfGameFieldBehavior> OutOfGameFieldBehaviors = Inc;
            public EcsPool<KillRequest> KillRequests = Opt;
        }

        public void Run()
        {
            foreach (var e in _world.Where(out Aspect a))
            {
                ref var rigidTransform = ref a.RigidTransforms[e];
                ref var boundsSphere = ref a.BoundsSpheres[e];
                ref var behavior = ref a.OutOfGameFieldBehaviors[e];
                Vector3 excess = CalculateExcess(rigidTransform.Position, boundsSphere.Radius);
                if (excess == Vector3.zero)
                {
                    continue;
                }

                switch (behavior.Mode)
                {
                    case OutOfGameFieldBehaviorMode.Clamp:
                        {
                            rigidTransform.Position -= excess;
                        }
                        break;
                    case OutOfGameFieldBehaviorMode.Wrap:
                        {
                            Vector3 position = rigidTransform.Position;
                            Vector2 fieldSize = r.FieldSize;

                            Vector3 gameFieldSize = new Vector3(fieldSize.x, 0, fieldSize.y) + 2f * boundsSphere.Radius * Vector3.one;


                            if (excess.x != 0)
                            {
                                position.x += -Mathf.Sign(excess.x) * gameFieldSize.x;
                            }
                            if (excess.z != 0)
                            {
                                position.z += -Mathf.Sign(excess.z) * gameFieldSize.z;
                            }

                            rigidTransform.Position = position;
                        }
                        break;
                    case OutOfGameFieldBehaviorMode.KillImmediate:
                        {
                            var fieldSize = r.FieldSize * c.AdditionalKillOffset;
                            if (CheckExcess(rigidTransform.Position, fieldSize))
                            {
                                _world.DelEntity(e);
                            }
                        }
                        break;
                    case OutOfGameFieldBehaviorMode.Kill:
                        {
                            var fieldSize = r.FieldSize * c.AdditionalKillOffset;
                            if (CheckExcess(rigidTransform.Position, fieldSize))
                            {
                                a.KillRequests.TryAddOrGet(e);
                            }
                        }
                        break;
                }


            }
        }

        private Vector3 CalculateExcess(Vector3 position, float radius)
        {
            Vector2 fieldSize = r.FieldSize;
            Vector3 fieldSizeHalf = new Vector3(fieldSize.x, 0, fieldSize.y) / 2f + Vector3.one * 2f * radius;
            Vector3 excess = position;

            excess.x = Mathf.Sign(excess.x) * CalculateAxisExcess(Mathf.Abs(excess.x), fieldSizeHalf.x);
            excess.y = Mathf.Sign(excess.y) * CalculateAxisExcess(Mathf.Abs(excess.y), fieldSizeHalf.y);
            excess.z = Mathf.Sign(excess.z) * CalculateAxisExcess(Mathf.Abs(excess.z), fieldSizeHalf.z);

            return excess;
        }

        private float CalculateAxisExcess(float axis, float sizeHalf)
        {
            return Mathf.Max(0, axis - sizeHalf);
        }

        private bool CheckExcess(Vector3 position, Vector2 fieldSize)
        {
            return 
                position.x <= -fieldSize.x / 2f || position.x > fieldSize.x / 2f || 
                position.z <= -fieldSize.y / 2f || position.z > fieldSize.y / 2f;
        }          
    }
}
