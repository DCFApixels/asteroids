using Asteroids.BoundsOverlapsFeature;
using Asteroids.Data;
using Asteroids.MovementFeature;
using DCFApixels.DragonECS;
using UnityEngine;

namespace Asteroids.GameFieldFueature
{
    [MetaGroup(GameFieldModule.META_GROUP)]
    [MetaColor(GameFieldModule.META_COLOR)]
    internal class WrapAroundGameFieldSystem : IEcsRun
    {
        [DI] EcsDefaultWorld _world;
        [DI] RuntimeData _runtimeData;

        private class Aspect : EcsAspect
        {
            public EcsPool<TransformData> TransformDatas = Inc;
            public EcsPool<BoundsSphere> BoundsSpheres = Inc;
            public EcsPool<InOutsideGameFieldSignal> InAroundGameFieldSignals = Inc;
            public EcsTagPool<WrapAroundGameFieldMarker> WrapAroundScreenMarkers = Inc;
        }

        public void Run()
        {
            foreach (var e in _world.Where(out Aspect a))
            {
                ref var transformData = ref a.TransformDatas[e];
                ref var boundsSphere = ref a.BoundsSpheres[e];
                ref var inAroundGameFieldSignal = ref a.InAroundGameFieldSignals[e];

                Vector3 position = transformData.position;
                Vector2 fieldSize = _runtimeData.FieldSize;

                Vector3 gameFieldCenter = Vector3.zero;
                Vector3 gameFieldSize = new Vector3(fieldSize.x, 0, fieldSize.y) + Vector3.one * 2f * boundsSphere.radius;


                if(inAroundGameFieldSignal.Arounds.x != 0)
                {
                    position.x += -Mathf.Sign(inAroundGameFieldSignal.Arounds.x) * gameFieldSize.x;
                }
                if(inAroundGameFieldSignal.Arounds.z != 0)
                {
                    position.z += -Mathf.Sign(inAroundGameFieldSignal.Arounds.z) * gameFieldSize.z;
                }


                transformData.position = position;
            }
        }
    }
}