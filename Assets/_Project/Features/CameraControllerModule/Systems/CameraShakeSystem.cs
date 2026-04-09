using DCFApixels.DragonECS;
using System.Linq;
using UnityEngine;

namespace Modules.CameraController
{

    [MetaGroup(CameraControllerModule.META_GROUP)]
    [MetaColor(CameraControllerModule.META_COLOR)]
    internal class CameraShakeSystem : IEcsRun, IEcsDefaultAddParams
    {
        public AddParams AddParams => EcsConsts.POST_END_LAYER;

        [DI] EcsDefaultWorld _world;
        [DI] CameraBrain _brain;

        class RequestAspect : EcsAspect
        {
            public EcsPool<CameraShakeRequest> Requests = Inc;
        }
        class Aspect : EcsAspect
        {
            public EcsPool<CameraShakeAction> Actions = Inc;
        }
        public void Run()
        {
            _world.GetAspects(out RequestAspect reqA, out Aspect a);
            foreach (var reqE in _world.Where(reqA))
            {
                ref var req = ref reqA.Requests[reqE];
                a.Actions.Add(reqE) = new()
                {
                    Strength = req.Strength,
                    Duration = req.Duration,
                    Time = req.Duration
                };
            }
            reqA.Requests.ClearAll();

            float sinStrength = 0f;
            float offsetStrength = 0f;

            foreach (var e in a.Actions.UpdateTime(Time.deltaTime))
            {
                _world.DelEntity(e);
            }
            foreach (var e in _world.Where(a))
            {
                ref var act = ref a.Actions[e];
                sinStrength += act.Strength * _brain.ShakeSinCurve.Evaluate(act.T);
                offsetStrength += act.Strength * _brain.ShakeOffsetCurve.Evaluate(act.T);
            }

            float sinMultiplier = CameraBrain.SoftSign(sinStrength * _brain.ShakeSmooth);
            Vector3 offset = _brain.ShakeAmplitude * sinMultiplier;
            Vector3 lp = CameraBrain.Sin(Time.time * _brain.ShakePeriod);
            lp = Vector3.Scale(lp, offset);

            lp += _brain.ShakeOffset * CameraBrain.SoftSign(offsetStrength * _brain.ShakeSmooth);

            _brain.ShakePivod.localPosition = lp;
        }
    }
}
