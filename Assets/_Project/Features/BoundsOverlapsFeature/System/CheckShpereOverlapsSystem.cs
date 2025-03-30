using Asteroids.Data;
using Asteroids.MovementFeature;
using DCFApixels.DragonECS;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Asteroids.BoundsOverlapsFeature
{
    [MetaGroup(BoundsOverlapsModule.META_GROUP)]
    [MetaColor(BoundsOverlapsModule.META_COLOR)]
    internal class CheckShpereOverlapsSystem : IEcsInit, IEcsRun
    {
        [DI] EntityGraph _graph;
        [DI] RuntimeData _runtimeData;

        List<AreaGrid2D<entlong>.Hit> _hits = new(64);
        Bucket<int>[] _sizeSortEntitiesBuckets = new Bucket<int>[64];

        private const float OFFSET_MULTIPLIER = 4f;

        public void Init()
        {
            for (int i = 0; i < _sizeSortEntitiesBuckets.Length; i++)
            {
                _sizeSortEntitiesBuckets[i] = new Bucket<int>(32, (1 << i) / OFFSET_MULTIPLIER * 2f);
            }
        }

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
            var relA = _graph.GraphWorld.GetAspect<RelAspect>();
            relA.OverlapsEvents.ClearAll();
            for (int i = 0; i < _sizeSortEntitiesBuckets.Length; i++)
            {
                _sizeSortEntitiesBuckets[i].Clear();
            }

            var es = _graph.World.WhereToGroup(out BoundsAspect a);
            foreach (var e in es)
            {
                ref var boundsSphere = ref a.BoundsSpheres[e];
                var index = NextPowerOfTwoExponent(boundsSphere.radius * OFFSET_MULTIPLIER);
                _sizeSortEntitiesBuckets[index].Add() = e;
            }

            for (int i = _sizeSortEntitiesBuckets.Length - 1; i >= 0; i--)
            {
                var sizeSortEntitiesBucket = _sizeSortEntitiesBuckets[i];
                foreach (var e in sizeSortEntitiesBucket)
                {
                    ref var boundsSphere = ref a.BoundsSpheres[e];
                    var position = a.TransformDatas[e].position;
                    _runtimeData.AreaGrid.FindAllInRadius(position.x, position.z, sizeSortEntitiesBucket.CheckedRadius, _hits);

                    foreach (var hit in _hits)
                    {
                        if (hit.Id.TryGetID(out var otherE) == false)
                        {
                            continue;
                        }

                        if (es.Has(otherE))
                        {
                            ref var otherBoundsSphere = ref a.BoundsSpheres[otherE];
                            var overlapRadius = otherBoundsSphere.radius + boundsSphere.radius;
                            if (boundsSphere.radius >= otherBoundsSphere.radius) //отсеиваем дублирование
                            {
                                if (hit.SqrDistance <= overlapRadius * overlapRadius && e != otherE)
                                {
                                    var relE = _graph.GetOrNewRelation(otherE, e);
                                    relA.OverlapsEvents.TryAddOrGet(relE);
                                    var relEInverse = _graph.GetOrNewInverseRelation(relE);
                                    relA.OverlapsEvents.TryAddOrGet(relEInverse);
                                }
                            }
                        }
                    }
                }
            }
        }



        public static unsafe int NextPowerOfTwoExponent(float v)
        {
            if (v < 1.0f) { return 0; } 

            uint bits;
            bits = *(uint*)&v;

            // Извлекаем экспоненту (8 бит) и преобразуем в смещенное значение
            int exponent = ((int)(bits >> 23) & 0xFF) - 127;
            // Извлекаем мантиссу (23 бита)
            uint mantissa = bits & 0x007FFFFF;
            // Если мантисса не нулевая, увеличиваем степень
            int result = exponent + (mantissa != 0 ? 1 : 0);

            return result > 63 ? 63 : (result < 0 ? 0 : result);
        }

        #region Bucket
        [DebuggerDisplay("Count: {Count}")]
        private class Bucket<T>
        {
            public readonly float CheckedRadius;
            public T[] _items;
            public int _count;

            public Bucket(int minSize, float checkedRadius)
            {
                CheckedRadius = checkedRadius;
                minSize = minSize < 4 ? 4 : NextPow2(minSize);
                _items = new T[minSize];
                _count = 0;
            }

            public int Count
            {
                get { return _count; }
            }
            public ref T this[int index]
            {
                get { return ref _items[index]; }
            }
            public ref T Add()
            {
                if(_items.Length <= _count)
                {
                    var newSize = NextPow2(_count + 1);
                    Array.Resize(ref _items, newSize);
                }
                return ref _items[_count++];
            }
            public void Clear()
            {
                _count = 0;
            }
            public Span<T>.Enumerator GetEnumerator()
            {
                return AsSpan().GetEnumerator();
            }
            public Span<T> AsSpan()
            {
                return new Span<T>(_items, 0, _count);
            }
            private static int NextPow2(int v)
            {
                unchecked
                {
                    v--;
                    v |= v >> 1;
                    v |= v >> 2;
                    v |= v >> 4;
                    v |= v >> 8;
                    v |= v >> 16;
                    return ++v;
                }
            }
        }
        #endregion
    }
}