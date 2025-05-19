using Asteroids.MovementFeature;
using DCFApixels;
using DCFApixels.DragonECS;
using UnityEngine;

namespace Asteroids.BoundsOverlapsFeature
{
    [MetaGroup(BoundsOverlapsModule.META_GROUP)]
    [MetaColor(BoundsOverlapsModule.META_COLOR)]
    internal class DebugCheckShpereOverlapsSystem : IEcsRun//, IEcsDefaultAddParams
    {
        [DI] EntityGraph _graph;

        //public AddParams AddParams => EcsConsts.POST_END_LAYER;

        class BoundsAspect : EcsAspect
        {
            public EcsPool<BoundsSphere> BoundsSpheres = Inc;
            public EcsPool<TransformData> TransformDatas = Inc;
        }
        class RelAspect : EcsAspect
        {
            public EcsPool<OverlapsEvent> OverlapsEvents = Inc;
        }

        public void Run()
        {
            var es = _graph.World.WhereToGroup(out BoundsAspect a);
            foreach (var relE in _graph.GraphWorld.Where(out RelAspect relA))
            {
                var (startE, endE) = _graph.GetRelationStartEnd(relE);
                if(es.Has(startE) && es.Has(endE))
                {
                    var pos1 = a.TransformDatas[startE].position;
                    var pos2 = a.TransformDatas[endE].position;
                    DebugX.Draw(new Color(0.5f, 1f, 0f)).Line(pos1, pos2);
                }
            }

            foreach (var e in es)
            {
                var pos1 = a.TransformDatas[e].position;
                var radius1 = a.BoundsSpheres[e].radius;
                DebugX.Draw(new Color(0.5f, 1f, 0f)).WireCircle(pos1, Vector3.up, radius1);
            }
        }
    }
}