using System.Collections.Generic;
using UnityEngine;

namespace Asteroids.Utils
{
    internal class PoolService
    {
        private readonly Dictionary<int, List<Component>> Pools = new();

        public void PreWarm(Component component, int amount)
        {
            var instanceID = component.GetInstanceID();
            if (!Pools.TryGetValue(instanceID, out var pool))
            {
                pool = new List<Component>();
                Pools.Add(instanceID, pool);
            }
            for (int i = 0; i < amount; i++)
            {
                var instance = Object.Instantiate(component);
                pool.Add(instance);
                instance.gameObject.SetActive(false);
            }
        }

        public void Return(int instanceID, Component component)
        {
            if (Pools.TryGetValue(instanceID, out var pool))
            {
                pool.Add(component);
                component.gameObject.SetActive(false);
            }
            else
            {
                Debug.LogError($"There is no pool for {component} with {instanceID}");
            }
        }

        public T Get<T>(T component, out int instanceID) where T : Component
        {
            instanceID = component.GetInstanceID();
            if (!Pools.TryGetValue(instanceID, out var pool))
            {
                pool = new List<Component>();
                Pools.Add(instanceID, pool);
            }

            if (pool.Count > 0)
            {
                var last = pool[^1];
                pool.RemoveAt(pool.Count-1);
                last.gameObject.SetActive(true);
                return (T)last;
            }
        
            var instance = Object.Instantiate(component);
            return instance;
        }
    }
}