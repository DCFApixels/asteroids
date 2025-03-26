using Asteroids.Common;
using Asteroids.Data;
using Asteroids.MovementFeature;
using DCFApixels.DragonECS;
using UnityEngine;

namespace Asteroids.GameFieldFueature
{
    internal class CheckAroundGameFieldSystem : IEcsRun
    {
        [DI] EcsDefaultWorld _world;
        [DI] RuntimeData _runtimeData;

        private class Aspect : EcsAspect
        {
            public EcsPool<TransformData> TransformDatas = Inc;
            public EcsPool<BoundsSphere> BoundsSpheres = Inc;
            public EcsPool<InOutsideGameFieldSignal> InAroundGameFieldSignals = Exc;
        }

        public void Run()
        {
            var a = _world.GetAspect<Aspect>();
            a.InAroundGameFieldSignals.ClearAll();

            foreach (var e in _world.Where(a))
            {
                ref var transformData = ref a.TransformDatas[e];
                ref var boundsSphere = ref a.BoundsSpheres[e];

                Vector3 position = transformData.position;
                Vector2 fieldSize = _runtimeData.FieldSize;

                Vector3 gameFieldCenter = Vector3.zero;
                Vector3 gameFieldSizeHalf = new Vector3(fieldSize.x, 0, fieldSize.y) / 2f + Vector3.one * 2f * boundsSphere.radius;
                Vector3 arounds = position - gameFieldCenter;


                float CalcAxisAround(float axis_, float sizeHalf_) { return Mathf.Max(0, axis_ - sizeHalf_); }
                arounds.x = Mathf.Sign(arounds.x) * CalcAxisAround(Mathf.Abs(arounds.x), gameFieldSizeHalf.x);
                arounds.y = Mathf.Sign(arounds.y) * CalcAxisAround(Mathf.Abs(arounds.y), gameFieldSizeHalf.y);
                arounds.z = Mathf.Sign(arounds.z) * CalcAxisAround(Mathf.Abs(arounds.z), gameFieldSizeHalf.z);

                //if (position.x < -fieldSize.x / 2f - boundsSphere.radius)
                //{
                //    position.x += fieldSize.x + 2 * boundsSphere.radius;
                //}
                //else if (position.x > fieldSize.x / 2f + boundsSphere.radius)
                //{
                //    position.x -= fieldSize.x + 2 * boundsSphere.radius;
                //}
                //
                //if (position.z < -fieldSize.y / 2f - boundsSphere.radius)
                //{
                //    position.z += fieldSize.y + 2 * boundsSphere.radius;
                //}
                //else if (position.z > fieldSize.y / 2f + boundsSphere.radius)
                //{
                //    position.z -= fieldSize.y + 2 * boundsSphere.radius;
                //}

                if (arounds != Vector3.zero)
                {
                    ref var inAroundGameFieldSignal = ref a.InAroundGameFieldSignals.Add(e);
                    inAroundGameFieldSignal.Arounds = arounds;
                }

                transformData.position = position;
            }
        }
    }
}