#if DISABLE_DEBUG
#undef DEBUG
#endif
using DCFApixels.DragonECS.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;


namespace DCFApixels.DragonECS
{
    /// <summary>Pool for IEcsRefComponent components</summary>
    [MetaColor(MetaColor.DragonRose)]
    [MetaGroup(EcsConsts.PACK_GROUP, EcsConsts.POOLS_GROUP)]
    [MetaDescription(EcsConsts.AUTHOR, "Pool for IEcsRefComponent components.")]
    [MetaID("DragonECS_109441DE9A017AB759542AA0C37BE180")]
    [DebuggerDisplay("Count: {Count} Ref: {ComponentType}")]
    public sealed class EcsRefPool<T> : IEcsPoolImplementation<T>, IEcsHybridPool<T>, IEnumerable<T>, IComponentMask //IEnumerable<T> - IntelliSense hack
        where T : class
    {
        private EcsWorld.ComponentsRegistrar _registrar;
        private readonly static EcsStaticMask _staticMask = EcsStaticMask.Inc<T>();

        private T[] _mapping;
        private int _count = 0;

        private bool _isInitInterfaces = false;
        private bool _isCustomLifecycle;
        private bool _isCustomCopy;

#if !DRAGONECS_DISABLE_POOLS_EVENTS
        private List<IEcsPoolEventListener> _listeners = new List<IEcsPoolEventListener>(2);
        private bool _hasAnyListener = false;
#endif
        private bool _isLocked;


        #region Properites
        public int Count
        {
            get { return _count; }
        }
        public int ComponentTypeID
        {
            get { return _registrar.ComponentTypeID; }
        }
        public Type ComponentType
        {
            get { return typeof(T); }
        }
        public EcsWorld World
        {
            get { return _registrar.World; }
        }
        public T this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return Get(index); }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set { Set(index, value); }
        }
        #endregion

        #region Constructors/Init/Destroy
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void CheckInitInterfaces(T obj)
        {
            if (_isInitInterfaces) { return; }
            InitInterfaces(obj);
        }
        [MethodImpl(MethodImplOptions.NoInlining)]
        private void InitInterfaces(T obj)
        {
            if (obj == null) { return; }
            _isCustomLifecycle = obj is IEcsComponentLifecycle<T> _customLifecycle;
            _isCustomCopy = obj is IEcsComponentCopy<T> _customCopy;
            _isInitInterfaces = true;
        }
        public EcsRefPool() { }
        void IEcsPoolImplementation.OnInit(EcsWorld.ComponentsRegistrar registrar)
        {
            _registrar = registrar;
            _mapping = new T[registrar.World.Capacity];
        }
        void IEcsPoolImplementation.OnWorldDestroy() { }
        #endregion

        #region Methods
        public void Add(int entityID, T obj)
        {
#if DEBUG
            if (entityID == EcsConsts.NULL_ENTITY_ID) { EcsPoolThrowHelper.ThrowEntityIsNotAlive(_registrar.World, entityID); }
            if (_registrar.World.IsUsed(entityID) == false) { EcsPoolThrowHelper.ThrowEntityIsNotAlive(_registrar.World, entityID); }
            if (ReferenceEquals(obj, null)) { EcsPoolThrowHelper.ThrowNullComponent(); }
            if (Has(entityID)) { EcsPoolThrowHelper.ThrowAlreadyHasComponent<T>(entityID); }
            if (_isLocked) { EcsPoolThrowHelper.ThrowPoolLocked(); }
#endif

#if DRAGONECS_STABILITY_MODE
            Set(entityID, obj);
#else
            Add_Internal(entityID, obj);
#endif
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Add_Internal(int entityID, T obj)
        {
            CheckInitInterfaces(obj);
            _count++;
            _mapping[entityID] = obj;
            _registrar.RegisterComponent(entityID);
            if (_isCustomLifecycle)
            {
                ((IEcsComponentLifecycle<T>)obj).OnAdd(ref obj, _registrar.WorldID, entityID);
            }
#if !DRAGONECS_DISABLE_POOLS_EVENTS
            if (_hasAnyListener) { _listeners.InvokeOnAddAndGet(entityID); }
#endif
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Get(int entityID)
        {
#if DEBUG // не нужен STAB_MODE
            if (!Has(entityID)) { EcsPoolThrowHelper.ThrowNotHaveComponent<T>(entityID); }
#endif
#if !DRAGONECS_DISABLE_POOLS_EVENTS
            if (_hasAnyListener) { _listeners.InvokeOnGet(entityID); }
#endif
            return _mapping[entityID];
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref readonly T Read(int entityID)
        {
#if DEBUG // не нужен STAB_MODE
            if (!Has(entityID)) { EcsPoolThrowHelper.ThrowNotHaveComponent<T>(entityID); }
#endif
            return ref _mapping[entityID];
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Has(int entityID)
        {
            return !ReferenceEquals(_mapping[entityID], null);
        }
        public void Del(int entityID)
        {
            //CheckInitInterfaces(obj);
#if DEBUG
            if (entityID == EcsConsts.NULL_ENTITY_ID) { EcsPoolThrowHelper.ThrowEntityIsNotAlive(_registrar.World, entityID); }
            if (!Has(entityID)) { EcsPoolThrowHelper.ThrowNotHaveComponent<T>(entityID); }
            if (_isLocked) { EcsPoolThrowHelper.ThrowPoolLocked(); }
#endif

#if !DRAGONECS_STABILITY_MODE
            Del_Internal(entityID);
#endif
        }
        private void Del_Internal(int entityID)
        {
            if (_isCustomLifecycle)
            {
                var obj = _mapping[entityID];
                ((IEcsComponentLifecycle<T>)obj).OnDel(ref obj, _registrar.WorldID, entityID);
            }
            _mapping[entityID] = null;
            _count--;
            _registrar.UnregisterComponent(entityID);
#if !DRAGONECS_DISABLE_POOLS_EVENTS
            if (_hasAnyListener) { _listeners.InvokeOnDel(entityID); }
#endif
        }
        public void TryDel(int entityID)
        {
            if (Has(entityID))
            {
                Del(entityID);
            }
        }
        public void Copy(int fromEntityID, int toEntityID)
        {
#if DEBUG
            if (!Has(fromEntityID)) { EcsPoolThrowHelper.ThrowNotHaveComponent<T>(fromEntityID); }
#elif DRAGONECS_STABILITY_MODE
            if (!Has(fromEntityID)) { return; }
#endif
            T result = null;
            ref var from = ref _mapping[fromEntityID];
            CheckInitInterfaces(from);
            if (_isCustomCopy)
            {
                ((IEcsComponentCopy<T>)from).Copy(ref from, ref result);
            }
            else
            {
                result = from;
            }
            Set(toEntityID, result);
        }
        public void Copy(int fromEntityID, EcsWorld toWorld, int toEntityID)
        {
#if DEBUG
            if (!Has(fromEntityID)) { EcsPoolThrowHelper.ThrowNotHaveComponent<T>(fromEntityID); }
#elif DRAGONECS_STABILITY_MODE
            if (!Has(fromEntityID)) { return; }
#endif
            T result = null;
            ref var from = ref _mapping[fromEntityID];
            CheckInitInterfaces(from);
            if (_isCustomCopy)
            {
                ((IEcsComponentCopy<T>)from).Copy(ref from, ref result);
            }
            else
            {
                result = from;
            }
            toWorld.GetPool<T>().Set(toEntityID, result);
        }
        public void Set(int entityID, T obj)
        {
#if DEBUG
            if (entityID == EcsConsts.NULL_ENTITY_ID) { EcsPoolThrowHelper.ThrowEntityIsNotAlive(_registrar.World, entityID); }
            if (_registrar.World.IsUsed(entityID) == false) { EcsPoolThrowHelper.ThrowEntityIsNotAlive(_registrar.World, entityID); }
            if (_isLocked) { EcsPoolThrowHelper.ThrowPoolLocked(); }
#elif DRAGONECS_STABILITY_MODE
            if (_isLocked) { return; }
#endif

            bool isHas = Has(entityID);
            if (ReferenceEquals(obj, null))
            {
                if (isHas)
                {
                    Del_Internal(entityID);
                }
            }
            else
            {
                if (isHas)
                {
                    _mapping[entityID] = obj;
                }
                else
                {
                    Add_Internal(entityID, obj);
                }
            }
        }
        public void ClearAll()
        {
            //CheckInitInterfaces(obj);
#if DEBUG
            if (_isLocked) { EcsPoolThrowHelper.ThrowPoolLocked(); }
#elif DRAGONECS_STABILITY_MODE
            if (_isLocked) { return; }
#endif
            if (_count <= 0) { return; }
            var span = _registrar.World.Where(out SinglePoolAspect<EcsRefPool<T>> _);
            _count = 0;
            foreach (var entityID in span)
            {
                if (_isCustomLifecycle)
                {
                    var obj = _mapping[entityID];
                    ((IEcsComponentLifecycle<T>)obj).OnDel(ref obj, _registrar.WorldID, entityID);
                }
                _mapping[entityID] = null;
                _registrar.UnregisterComponent(entityID);
#if !DRAGONECS_DISABLE_POOLS_EVENTS
                if (_hasAnyListener) { _listeners.InvokeOnDel(entityID); }
#endif
            }
        }
        #endregion

        #region Callbacks
        void IEcsPoolImplementation.OnWorldResize(int newSize)
        {
            Array.Resize(ref _mapping, newSize);
        }
        void IEcsPoolImplementation.OnReleaseDelEntityBuffer(ReadOnlySpan<int> buffer)
        {
            if (_count <= 0)
            {
                return;
            }
            foreach (var entityID in buffer)
            {
                TryDel(entityID);
            }
        }
        void IEcsPoolImplementation.OnLockedChanged_Debug(bool locked) { _isLocked = locked; }
        #endregion

        #region Other
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void ThrowNullComponent()
        {
            throw new ArgumentNullException("Component is null");
        }
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static int NextPow2Safe(int v, int min = 4)
        {
            return NextPow2(v < min ? min : v);
        }
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static int NextPow2(int v)
        {
            return CeilPow2(v | 1);
        }
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static int CeilPow2Safe(int v, int min = 4)
        {
            return CeilPow2(v < min ? min : v);
        }
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static int CeilPow2(int v)
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

        void IEcsPool.AddEmpty(int entityID) { Add(entityID, null); }
        void IEcsPool.AddRaw(int entityID, object dataRaw)
        {
            Add(entityID, dataRaw == null ? default : (T)dataRaw);
        }
        object IEcsReadonlyPool.GetRaw(int entityID) { return Read(entityID); }
        void IEcsPool.SetRaw(int entityID, object dataRaw)
        {
            Set(entityID, (T)dataRaw);
        }
        EcsMask IComponentMask.ToMask(EcsWorld world)
        {
            return _staticMask.ToMask(world);
        }
        #endregion

        #region Listeners
#if !DRAGONECS_DISABLE_POOLS_EVENTS
        public void AddListener(IEcsPoolEventListener listener)
        {
            if (listener == null) { EcsPoolThrowHelper.ThrowNullListener(); }
            _listeners.Add(listener);
            _hasAnyListener = _listeners.Count > 0;
        }
        public void RemoveListener(IEcsPoolEventListener listener)
        {
            if (listener == null) { EcsPoolThrowHelper.ThrowNullListener(); }
            if (_listeners.Remove(listener))
            {
                _hasAnyListener = _listeners.Count > 0;
            }
        }
#endif
        #endregion

        #region IEnumerator - IntelliSense hack
        IEnumerator<T> IEnumerable<T>.GetEnumerator() { throw new NotImplementedException(); }
        IEnumerator IEnumerable.GetEnumerator() { throw new NotImplementedException(); }
        #endregion

        #region Convertors
        public static implicit operator EcsRefPool<T>(IncludeMarker a) { return a.GetInstance<EcsRefPool<T>>(); }
        public static implicit operator EcsRefPool<T>(ExcludeMarker a) { return a.GetInstance<EcsRefPool<T>>(); }
        public static implicit operator EcsRefPool<T>(AnyMarker a) { return a.GetInstance<EcsRefPool<T>>(); }
        public static implicit operator EcsRefPool<T>(OptionalMarker a) { return a.GetInstance<EcsRefPool<T>>(); }
        public static implicit operator EcsRefPool<T>(EcsWorld.GetPoolInstanceMarker a) { return a.GetInstance<EcsRefPool<T>>(); }
        #endregion

        #region Apply
        public static void Apply(T component, int entityID, short worldID)
        {
            EcsWorld.GetPoolInstance<EcsRefPool<T>>(worldID).Set(entityID, component);
        }
        public static void Apply(T component, int entityID, EcsRefPool<T> pool)
        {
            pool.Set(entityID, component);
        }
        #endregion
    }

    [MetaTags(MetaTags.HIDDEN)]
    [MetaColor(MetaColor.DragonRose)]
    [MetaGroup(EcsConsts.PACK_GROUP, EcsConsts.OTHER_GROUP)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public readonly struct ReadonlyEcsRefPool<T> : IEcsReadonlyPool //IEnumerable<T> - IntelliSense hack
        where T : class
    {
        private readonly EcsRefPool<T> _pool;

        #region Properties
        public int ComponentTypeID
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return _pool.ComponentTypeID; }
        }
        public Type ComponentType
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return _pool.ComponentType; }
        }
        public EcsWorld World
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return _pool.World; }
        }
        public int Count
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return _pool.Count; }
        }
        public ref readonly T this[int entityID]
        {
            get { return ref _pool.Read(entityID); }
        }
        #endregion

        #region Constructors
        internal ReadonlyEcsRefPool(EcsRefPool<T> pool)
        {
            _pool = pool;
        }
        #endregion

        #region Methods
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Has(int entityID) { return _pool.Has(entityID); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref readonly T Get(int entityID) { return ref _pool.Read(entityID); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref readonly T Read(int entityID) { return ref _pool.Read(entityID); }
        object IEcsReadonlyPool.GetRaw(int entityID) { return _pool.Read(entityID); }

#if !DRAGONECS_DISABLE_POOLS_EVENTS
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddListener(IEcsPoolEventListener listener) { _pool.AddListener(listener); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RemoveListener(IEcsPoolEventListener listener) { _pool.AddListener(listener); }
#endif
        #endregion

        #region Convertors
        public static implicit operator ReadonlyEcsRefPool<T>(EcsRefPool<T> a) { return new ReadonlyEcsRefPool<T>(a); }
        public static implicit operator ReadonlyEcsRefPool<T>(IncludeMarker a) { return a.GetInstance<EcsRefPool<T>>(); }
        public static implicit operator ReadonlyEcsRefPool<T>(ExcludeMarker a) { return a.GetInstance<EcsRefPool<T>>(); }
        public static implicit operator ReadonlyEcsRefPool<T>(AnyMarker a) { return a.GetInstance<EcsRefPool<T>>(); }
        public static implicit operator ReadonlyEcsRefPool<T>(OptionalMarker a) { return a.GetInstance<EcsRefPool<T>>(); }
        public static implicit operator ReadonlyEcsRefPool<T>(EcsWorld.GetPoolInstanceMarker a) { return a.GetInstance<EcsRefPool<T>>(); }
        #endregion
    }

    public static class EcsRefPoolExtensions
    {
#if UNITY_2020_3_OR_NEWER
        public static void ClearDestroyedComponents<T>(this EcsRefPool<T> self)
            where T : UnityEngine.Object
        {
            foreach (var e in self.World.Where(out SinglePoolAspect<EcsRefPool<T>> _))
            {
                if (!self[e])
                {
                    self.Del(e);
                }
            }
        }
#endif
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static EcsRefPool<TComponent> GetPool<TComponent>(this EcsWorld self, bool _ignoreThisArg = false) where TComponent : class
        {
            return self.GetPoolInstance<EcsRefPool<TComponent>>();
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static EcsRefPool<TComponent> GetPoolUnchecked<TComponent>(this EcsWorld self, bool _ignoreThisArg = false) where TComponent : class
        {
            return self.GetPoolInstanceUnchecked<EcsRefPool<TComponent>>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static EcsRefPool<TComponent> Inc<TComponent>(this EcsAspect.Builder self, bool _ignoreThisArg = false) where TComponent : class
        {
            return self.IncludePool<EcsRefPool<TComponent>>();
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static EcsRefPool<TComponent> Exc<TComponent>(this EcsAspect.Builder self, bool _ignoreThisArg = false) where TComponent : class
        {
            return self.ExcludePool<EcsRefPool<TComponent>>();
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static EcsRefPool<TComponent> Opt<TComponent>(this EcsAspect.Builder self, bool _ignoreThisArg = false) where TComponent : class
        {
            return self.OptionalPool<EcsRefPool<TComponent>>();
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static EcsRefPool<TComponent> Any<TComponent>(this EcsAspect.Builder self, bool _ignoreThisArg = false) where TComponent : class
        {
            return self.AnyPool<EcsRefPool<TComponent>>();
        }
    }

#if UNITY_2021_2_OR_NEWER
    public abstract class RefComponentTemplate<T> : ComponentTemplateBase<T>
        where T : class
    {
        public override void Apply(short worldID, int entityID)
        {
            EcsRefPool<T>.Apply(component, entityID, worldID);
        }
    }
#endif
}