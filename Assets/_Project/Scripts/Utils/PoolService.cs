using Asteroids.Components;
using System.Collections.Generic;
using UnityEngine;

namespace Asteroids.Utils
{
    internal class PoolService
    {
        private readonly Dictionary<int, List<Component>> _pools = new();

        public void PreWarm(Component prefab, int amount)
        {
            var instanceID = prefab.GetInstanceID();
            if (!_pools.TryGetValue(instanceID, out var pool))
            {
                pool = new();
                _pools.Add(instanceID, pool);
            }
            for (var i = 0; i < amount; i++)
            {
                var instance = Object.Instantiate(prefab);
                pool.Add(instance);
                instance.gameObject.SetActive(false);
            }
        }

        public void Return(int instanceID, Component instance)
        {
            if (_pools.TryGetValue(instanceID, out var pool))
            {
                pool.Add(instance);
                instance.gameObject.SetActive(false);
            }
            else
            {
                Debug.LogError($"There is no pool for {instance} with {instanceID}");
            }
        }

        public T Get<T>(T prefab, out int instanceID) where T : Component
        {
            instanceID = prefab.GetInstanceID();
            if (!_pools.TryGetValue(instanceID, out var pool))
            {
                pool = new();
                _pools.Add(instanceID, pool);
            }

            if (pool.Count > 0)
            {
                var last = pool[^1];
                pool.RemoveAt(pool.Count-1);
                last.gameObject.SetActive(true);
                return (T)last;
            }
        
            var instance = Object.Instantiate(prefab);
            return instance;
        }
    }

    internal static class PoolServiceExtensions
    {
        public static T Get<T>(this PoolService self, T prefab, out PooledUnit pooledUnit) where T : Component
        {
            var result = self.Get<T>(prefab, out int id);
            pooledUnit.ID = id;
            pooledUnit.Unit = result;
            return result;
        }
    }
}