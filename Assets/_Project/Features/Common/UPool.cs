using System;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
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
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void StaticCleanup()
        {
            _pools.Clear();
        }
        private static readonly Dictionary<int, UPool> _pools = new Dictionary<int, UPool>(256);
        private static readonly List<int> _instIDsBuffer = new List<int>(32);
        public static void UnloadAllEmpty()
        {
            _instIDsBuffer.Clear();
            foreach (var (id, pool) in _pools)
            {
                if (pool.CheckEmpty())
                {
                    pool.OnUnload(true);
                    _instIDsBuffer.Add(id);
                }
            }
            foreach (var id in _instIDsBuffer)
            {
                _pools.Remove(id);
            }
        }
        public static void UnloadFor(UnityObject prefab, bool withDestroy)
        {
            if(TryGetExistPool(prefab, out UPool poolRaw))
            {
                poolRaw.OnUnload(withDestroy);
                _pools.Remove(prefab.GetInstanceID());
            }
        }
        public static bool TryGetExistPool(UnityObject prefabRaw, out UPool poolRaw)
        {
            int id = prefabRaw.GetInstanceID();
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
        public static UPool<T> GetFor<T>(T prefab, bool checkPrefab = true) where T : Component
        {
#if UNITY_EDITOR
            if (checkPrefab && UnityEditor.PrefabUtility.IsPartOfPrefabAsset(prefab.gameObject) == false) { Debug.LogWarning($"Соспавнен объект не из префаба. {prefab.name}"); }
#endif
            UPool<T> result = null;
            int id = prefab.GetInstanceID();
            bool createNew = true;
            if (_pools.TryGetValue(id, out UPool poolRaw))
            {
                result = UnsafeUtility.As<UPool, UPool<T>>(ref poolRaw);
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
    public static class UPoolExt 
    {
        public static void Despawn<T>(this T prefab, T instance)
            where T : Component
        {
            UPool.GetFor(prefab).Despawn(instance);
        }
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
        [SerializeField]
        private Transform _root;
        [SerializeField]
        private T _prefab;
        private IUPoolUnit<T> _prefabInterface;
        private readonly bool _isHasInterface;
        private readonly List<T> _pool = new List<T>(128);
        [SerializeField]
        private int _spawnedCount;
        public T Prefab
        {
            get { return _prefab; }
        }
        public override UnityObject PrefabRaw
        {
            get { return _prefab; }
        }
        protected override Transform Root
        {
            get { return _root; }
        }
        public override int PrewarmedCount
        {
            get { return _pool.Count; }
        }

        public UPool(Transform root, T prefab)
        {
            _root = root;
            _prefab = prefab;
            _prefabInterface = prefab as IUPoolUnit<T>;
            _isHasInterface = _prefabInterface != null;
            _isUnloaded = false;
            var debug = _root.gameObject.AddComponent<UPoolRoot>();
            debug.pool = this;

            if (_isHasInterface)
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
            if (_isHasInterface)
            {
                _prefabInterface.Static_InitPoolUnit(result, this);
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
            t.position = localPos;
            result.gameObject.SetActive(true);
            return result;
        }
        public T Spawn(Transform parent, Vector3 localPos, Quaternion localRot)
        {
            T result = TakeObject();
            var t = result.transform;
            t.SetParent(parent);
            t.SetPositionAndRotation(localPos, localRot);
            result.gameObject.SetActive(true);
            return result;
        }
        public T Spawn(Transform parent, Vector3 localPos, Quaternion localRot, Vector3 scale)
        {
            T result = TakeObject();
            var t = result.transform;
            t.SetParent(parent);
            t.localScale = scale;
            t.SetPositionAndRotation(localPos, localRot);
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
            if (_isHasInterface)
            {
                _prefabInterface.Static_ResetUnit(obj, _prefab);
            }
        }
    }
}