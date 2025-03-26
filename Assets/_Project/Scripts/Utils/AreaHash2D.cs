using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Asteroids.Utils
{
    public sealed class AreaHash2D<T>
    {
        private readonly List<Container>[] _area;
        private readonly float _invertedCellSize;
        private readonly float _minX;
        private readonly float _minY;
        private readonly int _areaX;
        private readonly int _areaY;

        private readonly List<List<Container>> _pool;
        private readonly List<List<Container>> _activeLists;

        private readonly int _startPoolSize;

        public AreaHash2D(float cellSize, float minX, float minY, float maxX, float maxY, int startPoolSize = 32)
        {
            _invertedCellSize = 1f / cellSize;
            _areaX = Ceiling((maxX - minX) * _invertedCellSize);
            _areaY = Ceiling((maxY - minY) * _invertedCellSize);

            _minX = minX;
            _minY = minY;
            _area = new List<Container>[_areaX * _areaY];
            _pool = new(_area.Length);
            _activeLists = new(_area.Length);

            _startPoolSize = startPoolSize;
        }
    
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int Ceiling(float value)
        {
            if (value == (int)value)
                return (int)value;
        
            if (value > 0)
                return (int)value + 1;
        
            return (int)value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(T id, float x, float y)
        {
            var hash = ToIndex(
                PositionToAreaIndex(x, _minX, _areaX),
                PositionToAreaIndex(y, _minY, _areaY));
            var area = _area[hash];
            if (area == null)
            {
                area = GetFromPool();
                _area[hash] = area;
                _activeLists.Add(area);
            }

            area.Add(new(id, x, y));
        }
    
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private List<Container> GetFromPool()
        {
            var count = _pool.Count;
            if (count <= 0)
            {
                return new(_startPoolSize);
            }
            
            count--;
            var l = _pool[count];
            _pool.RemoveAt(count);
            return l;
        }

        public void Clear()
        {
            Array.Clear(_area, 0, _area.Length);
            foreach (var list in _activeLists)
            {
                list.Clear();
                _pool.Add(list);
            }
            _activeLists.Clear();
        }

        public void FindAllInRadius(float xPos, float yPos, float radius, List<Hit> hits)
        {
            hits.Clear();

            var minX = PositionToAreaIndex(xPos - radius, _minX, _areaX);
            var minY = PositionToAreaIndex(yPos - radius, _minY, _areaY);
            var maxX = PositionToAreaIndex(xPos + radius, _minX, _areaX);
            var maxY = PositionToAreaIndex(yPos + radius, _minY, _areaY);
        
            var rSqr = radius * radius;
            for (var y = minY; y <= maxY; y++)
            {
                for (var x = minX; x <= maxX; x++)
                {
                    var list = _area[ToIndex(x, y)];
                    if (list == default)
                    {
                        continue;
                    }
                
                    foreach (var item in list)
                    {
                        var xDiff = xPos - item.X;
                        var yDiff = yPos - item.Y;
                        var sqrDistance = xDiff * xDiff + yDiff * yDiff;
                        if (!(sqrDistance <= rSqr))
                        {
                            continue;
                        }
                        
                        var hit = new Hit(item.Id, sqrDistance);
                        if (hits.Count == 0)
                        {
                            hits.Add(hit);
                        }
                        else
                        {
                            int index = 0, right = hits.Count - 1;

                            while (index <= right)
                            {
                                var mid = index + (right - index) / 2;

                                if (hits[mid].SqrDistance < sqrDistance)
                                {
                                    index = mid + 1;
                                }
                                else
                                {
                                    right = mid - 1;
                                }
                            }

                            if (index < 0) index = ~index;
                            hits.Insert(index, hit);
                        }
                    }
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int PositionToAreaIndex(float position, float min, int areaMax)
        {
            var index = (int)((position - min) * _invertedCellSize);
            if (index >= areaMax)
            {
                index = areaMax - 1;
            }
            else
            {
                if (index < 0)
                {
                    index = 0;
                }
            }

            return index;
        }
    
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int ToIndex(int x, int y)
        {
            return y * _areaX + x;
        }

        private struct Container
        {
            public readonly T Id;
            public readonly float X;
            public readonly float Y;

            public Container(T id, float x, float y)
            {
                Id = id;
                X = x;
                Y = y;
            }
        }
    
        public struct Hit
        {
            public readonly T Id;
            public readonly float SqrDistance;

            public Hit(T id, float sqrDistance)
            {
                Id = id;
                SqrDistance = sqrDistance;
            }
        }
    }
}



