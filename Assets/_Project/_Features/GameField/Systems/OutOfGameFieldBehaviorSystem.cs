using Asteroids.Components;
using Asteroids.GameFieldFeature;
using DCFApixels.DragonECS;
using Modules.BoundsOverlaps;
using Modules.Motion;
using UnityEngine;

namespace Asteroids.GameFieldFeature
{
    internal class OutOfGameFieldBehaviorSystem : IEcsRun
    {
        [DI] EcsDefaultWorld _world;
        [DI] RuntimeData r;
        [DI] ConfigData c;

        private class Aspect : EcsAspect
        {
            public EcsPool<RigidTransform> RigidTransforms = Inc;
            public EcsPool<BoundsSphere> BoundsSpheres = Inc;
            public EcsPool<OutOfGameFieldEvent> OutOfGameFieldEvents = Inc;
            public EcsPool<OutOfGameFieldBehavior> OutOfGameFieldBehaviors = Inc;
            public EcsPool<KillRequest> KillRequests = Opt;
        }

        public void Run()
        {
            foreach (var e in _world.Where(out Aspect a))
            {
                ref var rigidTransform = ref a.RigidTransforms[e];
                ref var boundsSphere = ref a.BoundsSpheres[e];
                ref var outOfGameFieldEvent = ref a.OutOfGameFieldEvents[e];
                ref var behavior = ref a.OutOfGameFieldBehaviors[e];

                switch (behavior.Mode)
                {
                    case OutOfGameFieldBehaviorMode.Clamp:
                        {
                            rigidTransform.Position -= outOfGameFieldEvent.Excess;
                        }
                        break;
                    case OutOfGameFieldBehaviorMode.Wrap:
                        {
                            Vector3 position = rigidTransform.Position;
                            Vector2 fieldSize = r.FieldSize;

                            Vector3 gameFieldSize = new Vector3(fieldSize.x, 0, fieldSize.y) + 2f * boundsSphere.Radius * Vector3.one;


                            if (outOfGameFieldEvent.Excess.x != 0)
                            {
                                position.x += -Mathf.Sign(outOfGameFieldEvent.Excess.x) * gameFieldSize.x;
                            }
                            if (outOfGameFieldEvent.Excess.z != 0)
                            {
                                position.z += -Mathf.Sign(outOfGameFieldEvent.Excess.z) * gameFieldSize.z;
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

        private bool CheckExcess(Vector3 position, Vector2 fieldSize)
        {
            return 
                position.x <= -fieldSize.x / 2f || position.x > fieldSize.x / 2f || 
                position.z <= -fieldSize.y / 2f || position.z > fieldSize.y / 2f;
        }          
    }
}