using Asteroids.Common;
using Asteroids.Components;
using Asteroids.Data;
using Asteroids.MovementFeature;
using Asteroids.Utils;
using DCFApixels.DragonECS;
using System;
using System.Collections.Generic;

namespace Asteroids.Systems
{
    internal class CheckShpereOverlapsSystem : IEcsInit, IEcsRun
    {
        [DI] EcsDefaultWorld _world;
        [DI] RuntimeData _runtimeData;

        List<AreaGrid2D<entlong>.Hit> _hits = new(64);
        Bucket<int>[] _sizeSortEntitiesBuckets = new Bucket<int>[64];

        public void Init()
        {
            for (int i = 0; i < _sizeSortEntitiesBuckets.Length; i++)
            {
                _sizeSortEntitiesBuckets[i] = new Bucket<int>(32, 1 << i);
            }
        }


        class BoundsAspect : EcsAspect
        {
            public EcsPool<BoundsSphere> BoundsSpheres = Inc;
            public EcsPool<TransformData> TransformDatas = Inc;
        }
        class EventAspect : EcsAspect
        {
            public EcsPool<OverlapsEvent> OverlapsEvents = Inc;
        }
        public void Run()
        {
            var eventA = _world.GetAspect<EventAspect>();
            eventA.OverlapsEvents.ClearAll();
            for (int i = 0; i < _sizeSortEntitiesBuckets.Length; i++)
            {
                _sizeSortEntitiesBuckets[i].Clear();
            }

            var es = _world.WhereToGroup(out BoundsAspect a);

            foreach (var e in es)
            {
                ref var boundsSphere = ref a.BoundsSpheres[e];
                var index = NextPowerOfTwoExponent(boundsSphere.radius);
                _sizeSortEntitiesBuckets[index].Add() = e;
            }


            for (int i = _sizeSortEntitiesBuckets.Length - 1; i >= 0; i--)
            {
                var sizeSortEntitiesBucket = _sizeSortEntitiesBuckets[i];
                foreach (var e in sizeSortEntitiesBucket)
                {
                    ref var boundsSphere = ref a.BoundsSpheres[e];
                    var position = a.TransformDatas[e].position;
                    _runtimeData.AreaHash.FindAllInRadius(position.x, position.z, sizeSortEntitiesBucket.CheckedRadius, _hits);

                    foreach (var hit in _hits)
                    {
                        var otherEntity = hit.Id;
                        if (otherEntity.TryGetID(out var otherE) == false)
                        {
                            continue;
                        }

                        if (es.Has(otherE))
                        {
                            ref var otherBoundsSphere = ref a.BoundsSpheres[otherE];
                            var overlapRadius = otherBoundsSphere.radius + boundsSphere.radius;
                            if (boundsSphere.radius > otherBoundsSphere.radius) //отсеиваем дублирование
                            {
                                if (hit.SqrDistance <= overlapRadius * overlapRadius && e != otherE)
                                {
                                    var eventE = _world.NewEntity(eventA);
                                    ref var overlapsEvent = ref eventA.OverlapsEvents[eventE];
                                    overlapsEvent.largestEntity = (_world, e);
                                    overlapsEvent.smallestEntity = otherEntity;
                                }
                            }
                        }
                    }
                }
            }
        }



        public static unsafe int NextPowerOfTwoExponent(float v)
        {
            if (v < 1.0f) { return 0; }      // Все значения < 1 → степень 0

            uint bits;
            bits = *(uint*)&v; // unsafe-преобразование float в uint

            // Извлекаем экспоненту (8 бит) и преобразуем в смещенное значение
            int exponent = ((int)(bits >> 23) & 0xFF) - 127;

            // Извлекаем мантиссу (23 бита)
            uint mantissa = bits & 0x007FFFFF;

            // Если мантисса не нулевая, увеличиваем степень
            int result = exponent + (mantissa != 0 ? 1 : 0);
            // Ограничиваем диапазон 0-63
            return result > 63 ? 63 : (result < 0 ? 0 : result);
        }

        #region Bucket
        private class Bucket<T>
        {
            public readonly float CheckedRadius;
            public T[] _items;
            public int _count;

            public Bucket(int minSize, float checkedRadius)
            {
                CheckedRadius = checkedRadius;
                if (minSize < 4)
                {
                    minSize = 4;
                }
                else
                {
                    minSize = NextPow2(minSize);
                }
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
                    var newSize = NextPow2(_count);
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