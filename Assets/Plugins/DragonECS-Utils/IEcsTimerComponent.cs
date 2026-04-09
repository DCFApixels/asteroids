#pragma warning disable UDR0001 // Domain Reload Analyzer
using DCFApixels.DragonECS.Core.Unchecked;
using System;
using UnityEngine;

namespace DCFApixels.DragonECS
{
    public interface IEcsTimerComponent
    {
        public float Time { get; set; }
    }
    public static class EcsTimerComponentExt
    {
        private static int[] _expiredEntitiesBuffer = new int[512];
        private static int _expiredEntitiesBufferCount = 0;
        public static bool UpdateTime<T>(this ref T cmp, float deltaTime)
            where T : struct, IEcsTimerComponent
        {
            if (cmp.Time > 0)
            {
                cmp.Time -= deltaTime;
            }
            return cmp.Time <= 0;
        }
        public static EcsSpan UpdateTime<T>(this EcsPool<T> pool, float deltaTime)
            where T : struct, IEcsComponent, IEcsTimerComponent
        {
            return pool.UpdateTime(pool.World.GetAspect<SingleAspect<T>>().Mask, deltaTime);
        }
        public static EcsSpan UpdateTime<T>(this EcsPool<T> pool, EcsMask mask, float deltaTime)
            where T : struct, IEcsComponent, IEcsTimerComponent
        {
            _expiredEntitiesBufferCount = 0;
            var span = pool.World.Where(mask);
            if (_expiredEntitiesBuffer.Length < span.Count)
            {
                Array.Resize(ref _expiredEntitiesBuffer, span.Count + 512);
            }
            foreach (var e in span)
            {
                ref var time = ref pool[e];
                //bool isExpired = time.Time <= 0f;
                time.Time -= deltaTime;
                //if(isExpired == false && time.Time <= 0f)
                if (time.Time <= 0f)
                {
                    _expiredEntitiesBuffer[_expiredEntitiesBufferCount++] = e;
                }
            }

            var newSpan = UncheckedUtility.CreateSpan(pool.World.ID, new ReadOnlySpan<int>(_expiredEntitiesBuffer, 0, _expiredEntitiesBufferCount));
#if DEBUG
            if (UncheckedUtility.CheckSpanValideDebug(newSpan) == false)
            {
                Debug.LogError("Span имеет дубликкты");
            }
#endif
            return newSpan;
        }
    }
    public interface IEcsCounterComponent
    {
        public int Count { get; set; }
    }

    public static class EcsCounterComponentExt
    {
        public static async Awaitable Increment<T>(this EcsPool<T> pool, int e, float duration) where T : struct, IEcsComponent, IEcsCounterComponent
        {
            if (duration <= 0) { return; }
            pool.Increment(e);
            await Awaitable.WaitForSecondsAsync(duration);
            pool.Decrement(e);
        }
        public static Scope<T> Increment<T>(this EcsPool<T> pool, int e) where T : struct, IEcsComponent, IEcsCounterComponent
        {
            pool.TryAddOrGet(e).Count++;
            return new Scope<T>(pool, e);
        }
        public static void Decrement<T>(this EcsPool<T> pool, int e) where T : struct, IEcsComponent, IEcsCounterComponent
        {
            if (pool.Has(e))
            {
                ref var c = ref pool.Get(e);
                c.Count--;
                if (c.Count == 0)
                {
                    pool.Del(e);
                }
                if (c.Count < 0)
                {
                    throw new System.Exception("Increment/Decrement balans exception");
                }
            }
        }

        public struct Scope<T> : IDisposable where T : struct, IEcsComponent, IEcsCounterComponent
        {
            public EcsPool<T> Pool;
            public int E;
            public Scope(EcsPool<T> pool, int e)
            {
                Pool = pool;
                E = e;
            }
            void IDisposable.Dispose()
            {
                Pool.Decrement(E);
            }
        }
    }
}
