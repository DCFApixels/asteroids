using Asteroids.MovementFeature;
using DCFApixels.DragonECS;
using Modules.BoundsOverlaps;
using UnityEngine;

namespace Asteroids.GameFieldFueature
{
    [MetaGroup(GameFieldModule.META_GROUP)]
    [MetaColor(GameFieldModule.META_COLOR)]
    internal class OutOfGameFieldDetectionSystem : IEcsRun
    {
        [DI] EcsDefaultWorld _world;
        [DI] RuntimeData r;

        private class Aspect : EcsAspect
        {
            public EcsPool<RigidTransform> TransformDatas = Inc;
            public EcsPool<BoundsSphere> BoundsSpheres = Inc;
            public EcsPool<OutOfGameFieldEvent> OutOfGameFieldEvents = Exc;
        }

        public void Run()
        {
            var a = _world.GetAspect<Aspect>();
            a.OutOfGameFieldEvents.ClearAll();

            foreach (var e in _world.Where(a))
            {
                ref var transformData = ref a.TransformDatas[e];
                ref var boundsSphere = ref a.BoundsSpheres[e];

                Vector3 position = transformData.Position;
                Vector2 fieldSize = r.FieldSize;

                Vector3 gameFieldCenter = Vector3.zero;
                Vector3 gameFieldSizeHalf = new Vector3(fieldSize.x, 0, fieldSize.y) / 2f + Vector3.one * 2f * boundsSphere.Radius;
                Vector3 outside = position - gameFieldCenter;

                float CalcAxisOutside(float axis_, float sizeHalf_) { return Mathf.Max(0, axis_ - sizeHalf_); }
                outside.x = Mathf.Sign(outside.x) * CalcAxisOutside(Mathf.Abs(outside.x), gameFieldSizeHalf.x);
                outside.y = Mathf.Sign(outside.y) * CalcAxisOutside(Mathf.Abs(outside.y), gameFieldSizeHalf.y);
                outside.z = Mathf.Sign(outside.z) * CalcAxisOutside(Mathf.Abs(outside.z), gameFieldSizeHalf.z);

                if (outside != Vector3.zero)
                {
                    ref var inOutsideGameFieldSignal = ref a.OutOfGameFieldEvents.Add(e);
                    inOutsideGameFieldSignal.Excess = outside;
                }

                transformData.Position = position;
            }
        }
    }
}