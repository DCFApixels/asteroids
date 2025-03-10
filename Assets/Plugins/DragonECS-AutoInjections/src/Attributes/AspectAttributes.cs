﻿using DCFApixels.DragonECS.PoolsCore;
using System;
using System.Linq;

namespace DCFApixels.DragonECS
{
    public abstract class InjectAspectMemberAttribute : Attribute { }
    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public sealed class IncAttribute : InjectAspectMemberAttribute { }
    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public sealed class ExcAttribute : InjectAspectMemberAttribute { }
    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public sealed class OptAttribute : InjectAspectMemberAttribute { }
    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public sealed class CombineAttribute : InjectAspectMemberAttribute
    {
        public readonly int order = 0;
        public CombineAttribute(int order = 0) { this.order = order; }
    }

    public abstract class ImplicitInjectAttribute : Attribute { }
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public sealed class IncImplicitAttribute : ImplicitInjectAttribute
    {
        public readonly Type type;
        public readonly bool isPool;
        public IncImplicitAttribute(Type type)
        {
            this.type = type;
            isPool = type.GetInterfaces().Any(o => o == typeof(IEcsPoolImplementation));
        }
    }
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public sealed class ExcImplicitAttribute : ImplicitInjectAttribute
    {
        public readonly Type type;
        public readonly bool isPool;
        public ExcImplicitAttribute(Type type)
        {
            this.type = type;
            isPool = type.GetInterfaces().Any(o => o == typeof(IEcsPoolImplementation));
        }
    }
}

