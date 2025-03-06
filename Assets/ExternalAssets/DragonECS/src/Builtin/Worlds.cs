﻿using System.Diagnostics;

namespace DCFApixels.DragonECS
{
    /// <summary> EcsWrold for store regular game entities. </summary>
    [MetaColor(MetaColor.DragonRose)]
    [MetaGroup(EcsConsts.PACK_GROUP, EcsConsts.WORLDS_GROUP)]
    [MetaDescription(EcsConsts.AUTHOR, "Inherits EcsWorld without extending its functionality and is used for specific injections. Can be used to store regular game entities, can also be used as a single world in the game for all entities.")]
    [DebuggerTypeProxy(typeof(DebuggerProxy))]
    [MetaID("4EE3527C92015BAB0299CB7B4E2663D1")]
    public sealed class EcsDefaultWorld : EcsWorld, IInjectionUnit
    {
        public EcsDefaultWorld() : base() { }
        public EcsDefaultWorld(EcsWorldConfig config = null, string name = null, short worldID = -1) : base(config, name == null ? "Default" : name, worldID) { }
        public EcsDefaultWorld(IConfigContainer configs, string name = null, short worldID = -1) : base(configs, name == null ? "Default" : name, worldID) { }
        void IInjectionUnit.InitInjectionNode(InjectionGraph nodes) { nodes.AddNode(this); }
    }
    /// <summary> EcsWrold for store event entities. </summary>
    [MetaColor(MetaColor.DragonRose)]
    [MetaGroup(EcsConsts.PACK_GROUP, EcsConsts.WORLDS_GROUP)]
    [MetaDescription(EcsConsts.AUTHOR, "Inherits EcsWorld without extending its functionality and is used for specific injections. Can be used to store event entities.")]
    [DebuggerTypeProxy(typeof(DebuggerProxy))]
    [MetaID("D7CE527C920160BCD765EFA72DBF8B89")]
    public sealed class EcsEventWorld : EcsWorld, IInjectionUnit
    {
        public EcsEventWorld() : base() { }
        public EcsEventWorld(EcsWorldConfig config = null, string name = null, short worldID = -1) : base(config, name == null ? "Events" : name, worldID) { }
        public EcsEventWorld(IConfigContainer configs, string name = null, short worldID = -1) : base(configs, name == null ? "Events" : name, worldID) { }
        void IInjectionUnit.InitInjectionNode(InjectionGraph nodes) { nodes.AddNode(this); }
    }
}
