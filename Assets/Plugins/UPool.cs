using System;
using System.Collections.Generic;
using UnityEngine;
using UnityObject = UnityEngine.Object;

#if UNITY_EDITOR
namespace DCFApixels.Editors
{
    using UnityEditor;

    [CanEditMultipleObjects]
    [CustomEditor(typeof(UPoolRoot))]
    public class UPoolRootDrawer : Editor
    {
        public override void OnInspectorGUI()
        {
            if(targets.Length > 1)
            {
                base.OnInspectorGUI();
                return;
            }

            UPool pool = ((UPoolRoot)target).pool;
            if (pool == null) { return; }

            EditorGUILayout.ObjectField("Prefab", pool.PrefabRaw, typeof(UnityObject), true);
            EditorGUILayout.IntField("PrewarmedCount", pool.PrewarmedCount);
        }
    }
}
#endif

namespace DCFApixels
{
    public interface IUPoolUnit<T>
    {
        void Static_InitPool(UPool pool);
        void Static_InitPoolUnit(T self, UPool pool);
        void Static_ResetUnit(T self, T prefab);
    }
    internal class UPoolRoot : MonoBehaviour
    {
        [System.NonSerialized]
        public UPool pool;
    }

    [Serializable]
    public abstract class UPool
    {
        private readonly struct UPoolObjectId : IEquatable<UPoolObjectId>
        {
#if UNITY_6000_2_OR_NEWER
            private readonly EntityId _value;

            private UPoolObjectId(EntityId value)
#else
            private readonly int _value;

            private UPoolObjectId(int value)
#endif
            {
                _value = value;
            }
            public static UPoolObjectId From(UnityObject obj)
            {
#if UNITY_6000_2_OR_NEWER
                return new UPoolObjectId(obj.GetEntityId());
#else
                return new UPoolObjectId(obj.GetInstanceID());
#endif
            }
            public bool Equals(UPoolObjectId other) { return _value.Equals(other._value); }
            public override bool Equals(object obj) { return obj is UPoolObjectId other && Equals(other); }
            public override int GetHashCode() { return _value.GetHashCode(); }
            public override string ToString() { return _value.ToString(); }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void StaticCleanup()
        {
            _pools.Clear();
        }
        private static readonly Dictionary<UPoolObjectId, UPool> _pools = new Dictionary<UPoolObjectId, UPool>(256);
        private static readonly List<UPoolObjectId> _idsBuffer = new List<UPoolObjectId>(32);
        public static void UnloadAllEmpty()
        {
            _idsBuffer.Clear();
            foreach (var (id, pool) in _pools)
            {
                if (pool.CheckEmpty())
                {
                    pool.OnUnload(true);
                    _idsBuffer.Add(id);
                }
            }
            foreach (var id in _idsBuffer)
            {
                _pools.Remove(id);
            }
        }
        public static void UnloadFor(UnityObject prefab, bool withDestroy)
        {
            if(TryGetExistPool(prefab, out UPool poolRaw))
            {
                poolRaw.OnUnload(withDestroy);
                _pools.Remove(UPoolObjectId.From(prefab));
            }
        }
        public static bool TryGetExistPool(UnityObject prefabRaw, out UPool poolRaw)
        {
            UPoolObjectId id = UPoolObjectId.From(prefabRaw);
            return _pools.TryGetValue(id, out poolRaw);
        }
        public static bool TryGetExistPool<T>(T prefab, out UPool<T> pool) where T : Component
        {
            if (TryGetExistPool(prefab, out UPool poolRaw))
            {
                pool = (UPool<T>)poolRaw;
                return true;
            }
            pool = null;
            return false;
        }
        public static UPool<T> GetFor<T>(T prefab) where T : Component
        {
            UPool<T> result = null;
            UPoolObjectId id = UPoolObjectId.From(prefab);
            bool createNew = true;
            if (_pools.TryGetValue(id, out UPool poolRaw))
            {
                result = (UPool<T>)poolRaw;
                createNew = !result.Root;
                if (createNew)
                {
                    result.OnUnload(true);
                    _pools.Remove(id);
                }
            }
            if(createNew)
            {
                Transform root = new GameObject(GenerateRootName(prefab)).transform;
                result = new UPool<T>(root, prefab);
                _pools.Add(id, result);
            }
            return result;
        }
        private static string GenerateRootName(UnityObject prefab)
        {
#if DEBUG
            return $"(UPool){prefab.name}";
#else
            return prefab.name;
#endif
        }

        public static UPool<T> Init<T>(ref UPool<T> pool, T prefab) where T : Component
        {
            if(pool != null)
            {
#if DEBUG
                if (pool.PrefabRaw != prefab) { throw new Exception("Префаб не соответсвует префабу который проинициализирован пулл"); }
#endif
                return pool;
            }

            pool = GetFor(prefab);
            return pool;
        }


        [SerializeField]
        protected bool _isUnloaded = false;
        public bool IsUnloaded
        {
            get
            {
                return Root == null ||  _isUnloaded;
            }
        }

        public abstract UnityObject PrefabRaw { get; }
        protected abstract Transform Root { get; }
        public abstract int PrewarmedCount { get; }
        public abstract bool CheckEmpty();
        public abstract void Prewarm(int count);
        public abstract void DontDestroyOnLoad();
        public abstract void DespawnRaw(UnityObject obj);
        protected abstract void OnUnload(bool withDestroy);
    }
    [Serializable]
    public struct UPrefab<T> where T : Component
    {
        [SerializeField]
        private T _prefab;
        [NonSerialized]
        private UPool<T> _pool;
        public UPrefab(T prefab)
        {
            _prefab = prefab;
            _pool = null;
        }
        public T Value { get { return _prefab; } }
        public bool IsAssigned { get { return _prefab; } }
        public UPool<T> Pool { get { return GetPool(); } }

        public UPool<T> GetPool()
        {
            if (!_prefab)
            {
                throw new InvalidOperationException($"{typeof(T).Name} prefab is missing.");
            }
            if (_pool == null || _pool.IsUnloaded || _pool.Prefab != _prefab)
            {
                _pool = UPool.GetFor(_prefab);
            }
            return _pool;
        }

        public bool TryGetExistingPool(out UPool<T> pool)
        {
            if (_pool != null && !_pool.IsUnloaded && _pool.Prefab == _prefab)
            {
                pool = _pool;
                return true;
            }
            if (!_prefab)
            {
                pool = null;
                return false;
            }
            if (UPool.TryGetExistPool(_prefab, out pool))
            {
                _pool = pool;
                return true;
            }
            return false;
        }
        public void Prewarm(int count) { GetPool().Prewarm(count); }
        public void DontDestroyOnLoad() { GetPool().DontDestroyOnLoad(); }

        public void Unload(bool withDestroy = true)
        {
            if (!_prefab)
            {
                _pool = null;
                return;
            }
            UPool.UnloadFor(_prefab, withDestroy);
            _pool = null;
        }
        public T Spawn(Transform parent = null)
        {
            return GetPool().Spawn(parent);
        }
        public T Spawn(Transform parent, Vector3 localPos)
        {
            return GetPool().Spawn(parent, localPos);
        }
        public T Spawn(Transform parent, Vector3 localPos, Quaternion localRot)
        {
            return GetPool().Spawn(parent, localPos, localRot);
        }
        public T Spawn(Transform parent, Vector3 localPos, Quaternion localRot, Vector3 scale)
        {
            return GetPool().Spawn(parent, localPos, localRot, scale);
        }
        public UPoolSpawnScope SpawnTemp(out T unit, Transform parent = null)
        {
            return GetPool().SpawnTemp(out unit, parent);
        }
        public UPoolSpawnScope SpawnTemp(out T unit, Transform parent, Vector3 localPos)
        {
            return GetPool().SpawnTemp(out unit, parent, localPos);
        }
        public UPoolSpawnScope SpawnTemp(out T unit, Transform parent, Vector3 localPos, Quaternion localRot)
        {
            return GetPool().SpawnTemp(out unit, parent, localPos, localRot);
        }

        public void Despawn(T instance)
        {
            if (!instance) { return; }
            GetPool().Despawn(instance);
        }
        public override string ToString()
        {
            return _prefab ? _prefab.name : "None";
        }
        public static implicit operator UPrefab<T>(T prefab)
        {
            return new UPrefab<T>(prefab);
        }
    }
    public static class UPoolExt
    {

        public static T Spawn<T>(this T prefab, Transform parent = null)
            where T : Component
        {
            return UPool.GetFor(prefab).Spawn(parent);
        }
        public static T Spawn<T>(this T prefab, Transform parent, Vector3 pos)
            where T : Component
        {
            return UPool.GetFor(prefab).Spawn(parent, pos);
        }
        public static T Spawn<T>(this T prefab, Transform parent, Vector3 pos, Quaternion rot)
            where T : Component
        {
            return UPool.GetFor(prefab).Spawn(parent, pos, rot);
        }
        public static T Spawn<T>(this T prefab, Transform parent, Vector3 pos, Quaternion rot, Vector3 scale)
            where T : Component
        {
            return UPool.GetFor(prefab).Spawn(parent, pos, rot, scale);
        }
        public static UPoolSpawnScope SpawnTemp<T>(this T prefab, out T unit, Transform parent = null)
            where T : Component
        {
            return UPool.GetFor(prefab).SpawnTemp(out unit, parent);
        }
        public static UPoolSpawnScope SpawnTemp<T>(this T prefab, out T unit, Transform parent, Vector3 pos)
            where T : Component
        {
            return UPool.GetFor(prefab).SpawnTemp(out unit, parent, pos);
        }
        public static UPoolSpawnScope SpawnTemp<T>(this T prefab, out T unit, Transform parent, Vector3 pos, Quaternion rot)
            where T : Component
        {
            return UPool.GetFor(prefab).SpawnTemp(out unit, parent, pos, rot);
        }


        //Temporary 
    }
    public readonly struct UPoolSpawnScope : IDisposable
    {
        public readonly UPool Pool;
        public readonly UnityObject Unit;
        public UPoolSpawnScope(UPool pool, UnityObject unit)
        {
            Pool = pool;
            Unit = unit;
        }
        public void Dispose()
        {
            Pool.DespawnRaw(Unit);
        }
        public async void Duration(float seconds)
        {
            await DurationAwaitable(seconds);
        }
        public async Awaitable DurationAwaitable(float seconds)
        {
            await Awaitable.WaitForSecondsAsync(seconds);
            Dispose();
        }
    }
    [Serializable]
    public sealed class UPool<T> : UPool where T : Component
    {
        private interface IUPoolUnitProxy
        {
            void Static_InitPool(UPool pool);
            void Static_InitPoolUnit(T self, UPool pool);
            void Static_ResetUnit(T self, T prefab);
        }

        private sealed class UPoolUnitProxy<TUnit> : IUPoolUnitProxy where TUnit : Component
        {
            private readonly IUPoolUnit<TUnit> _prefabInterface;

            public UPoolUnitProxy(IUPoolUnit<TUnit> prefabInterface)
            {
                _prefabInterface = prefabInterface;
            }

            public void Static_InitPool(UPool pool)
            {
                _prefabInterface.Static_InitPool(pool);
            }

            public void Static_InitPoolUnit(T self, UPool pool)
            {
                _prefabInterface.Static_InitPoolUnit((TUnit)(Component)self, pool);
            }

            public void Static_ResetUnit(T self, T prefab)
            {
                _prefabInterface.Static_ResetUnit((TUnit)(Component)self, (TUnit)(Component)prefab);
            }
        }

        private static IUPoolUnitProxy TryCreatePoolUnitProxy(T prefab)
        {
            if (prefab is IUPoolUnit<T> directInterface)
            {
                return new UPoolUnitProxy<T>(directInterface);
            }

            Type prefabType = prefab.GetType();
            Type unitType = typeof(T).BaseType;
            while (unitType != null && typeof(Component).IsAssignableFrom(unitType))
            {
                Type interfaceType = typeof(IUPoolUnit<>).MakeGenericType(unitType);
                if (interfaceType.IsAssignableFrom(prefabType))
                {
                    Type proxyType = typeof(UPoolUnitProxy<>).MakeGenericType(unitType);
                    return (IUPoolUnitProxy)Activator.CreateInstance(proxyType, prefab);
                }
                unitType = unitType.BaseType;
            }
            return null;
        }
        [SerializeField]
        private Transform _root;
        [SerializeField]
        private T _prefab;
        private IUPoolUnitProxy _prefabInterface;
        private readonly List<T> _pool = new List<T>(128);
        [SerializeField]
        private int _spawnedCount;
        public T Prefab { get { return _prefab; } }
        public override UnityObject PrefabRaw { get { return _prefab; } }
        protected override Transform Root { get { return _root; } }
        public override int PrewarmedCount { get { return _pool.Count; } }

        public UPool(Transform root, T prefab)
        {
            _root = root;
            _prefab = prefab;
            _prefabInterface = TryCreatePoolUnitProxy(prefab);
            _isUnloaded = false;
            var debug = _root.gameObject.AddComponent<UPoolRoot>();
            debug.pool = this;

            if (_prefabInterface != null)
            {
                _prefabInterface.Static_InitPool(this);
            }
        }
        public void Validate(Transform root)
        {
            CleanupDestroyedGOs();
            _root = root;
            var debug = _root.gameObject.AddComponent<UPoolRoot>();
            debug.pool = this;
        }
        public override void Prewarm(int count)
        {
            CleanupDestroyedGOs();
            int starCount = _pool.Count;
            for (int i = 0; i < count; i++)
            {
                T obj = Create();
                obj.gameObject.SetActive(false);
                _pool.Add(obj);
            }
            for (int i = starCount; i < _pool.Count; i++)
            {
                var obj = _pool[i];
            }
        }
        public override void DontDestroyOnLoad()
        {
            UnityObject.DontDestroyOnLoad(_root.gameObject);
        }
        private T Create()
        {
            T result = UnityObject.Instantiate(_prefab, _root);
            if (_prefabInterface != null)
            {
                _prefabInterface.Static_InitPoolUnit(result, this);
            }
            return result;
        }

        public T TakeObject()
        {
            T result;
            if (_pool.Count > 0)
            {
                int index = _pool.Count - 1;
                result = _pool[index];
                _pool.RemoveAt(index);
            }
            else
            {
                result = Create();
            }
            _spawnedCount++;
            return result;
        }
        public T Spawn(Transform parent = null)
        {
            T result = TakeObject();
            result.transform.SetParent(parent);
            result.gameObject.SetActive(true);
            return result;
        }
        public T Spawn(Transform parent, Vector3 localPos)
        {
            T result = TakeObject();
            var t = result.transform;
            t.SetParent(parent);
            t.localPosition = localPos;
            result.gameObject.SetActive(true);
            return result;
        }
        public T Spawn(Transform parent, Vector3 localPos, Quaternion localRot)
        {
            T result = TakeObject();
            var t = result.transform;
            t.SetParent(parent);
            t.SetLocalPositionAndRotation(localPos, localRot);
            result.gameObject.SetActive(true);
            return result;
        }
        public T Spawn(Transform parent, Vector3 localPos, Quaternion localRot, Vector3 scale)
        {
            T result = TakeObject();
            var t = result.transform;
            t.SetParent(parent);
            t.localScale = scale;
            t.SetLocalPositionAndRotation(localPos, localRot);
            result.gameObject.SetActive(true);
            return result;
        }
        public UPoolSpawnScope SpawnTemp(out T unit, Transform parent = null)
        {
            unit = Spawn(parent);
            return new UPoolSpawnScope(this, unit);
        }
        public UPoolSpawnScope SpawnTemp(out T unit, Transform parent, Vector3 localPos)
        {
            unit = Spawn(parent, localPos);
            return new UPoolSpawnScope(this, unit);
        }
        public UPoolSpawnScope SpawnTemp(out T unit, Transform parent, Vector3 localPos, Quaternion localRot)
        {
            unit = Spawn(parent, localPos, localRot);
            return new UPoolSpawnScope(this, unit);
        }
        public override void DespawnRaw(UnityObject obj)
        {
            Despawn((T)obj);
        }
        protected override void OnUnload(bool withDestroy)
        {
            if (withDestroy)
            {
                foreach (var unit in _pool)
                {
                    if (unit)
                    {
                        UnityObject.Destroy(unit.gameObject);
                    }
                }
            }
            _pool.Clear();
            if (_root)
            {
                UnityObject.Destroy(_root.gameObject);
            }
            _root = null;
            _prefab = null;
            _prefabInterface = null;
            _isUnloaded = true;
        }
        public override bool CheckEmpty()
        {
            if(_pool.Count <= 0)
            {
                return true;
            }
            foreach (var unit in _pool)
            {
                if (unit) { return false; }
            }
            return true;
        }
        private void CleanupDestroyedGOs()
        {
            for (int count = _pool.Count, i = count - 1; i >= 0; i--)
            {
                if (!_pool[i])
                {
                    count--;
                    _pool[i] = _pool[count];
                    _pool[count] = null;
                }
            }
        }
        public void Despawn(T obj)
        {
            if (!obj) { return; }
            if (_isUnloaded)
            {
                Debug.LogWarning($"Пул для {typeof(T).Name} уже выгружен");
                return;
            }
            _spawnedCount--;
            obj.transform.SetParent(_root);
            obj.gameObject.SetActive(false);
            _pool.Add(obj);
            if (_prefabInterface != null)
            {
                _prefabInterface.Static_ResetUnit(obj, _prefab);
            }
        }
    }
}
#if UNITY_EDITOR
namespace DCFApixels.Editors
{
    using UnityEditor;
    using UnityEngine;

    [CustomPropertyDrawer(typeof(UPrefab<>))]
    internal sealed class UPrefabDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty prefabProperty = property.FindPropertyRelative("_prefab");
            EditorGUI.PropertyField(position, prefabProperty, label, false);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            SerializedProperty prefabProperty = property.FindPropertyRelative("_prefab");
            return EditorGUI.GetPropertyHeight(prefabProperty, label, false);
        }
    }
}
#endif
