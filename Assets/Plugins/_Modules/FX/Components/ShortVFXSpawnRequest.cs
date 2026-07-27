using DCFApixels.DragonECS;
using DCFApixels.DragonECS.Core;
using UnityEngine;
using Utils;

namespace Modules.FX
{
    [System.Serializable]
    public struct ShortVFXSpawnRequest : IEcsComponent, IEcsComponentLifecycle<ShortVFXSpawnRequest>
    {
        public readonly static ShortVFXSpawnRequest Default = new()
        {
            Position = Vector3.zero,
            Rotation = Quaternion.identity,
            Scale = Nlb<float>.Manual(1f, true),
            Color = Nlb<Color>.Manual(UnityEngine.Color.white, true),
        };
        public ShortVFXView Prefab;
        public Vector3 Position;
        public Quaternion Rotation;
        public Nlb<float> Scale;
        public Nlb<Color> Color;
        public Nlb<Vector3> Direction;

        void IEcsComponentLifecycle<ShortVFXSpawnRequest>.OnAdd(ref ShortVFXSpawnRequest component, short worldID, int entityID)
        {
            component = Default;
        }
        void IEcsComponentLifecycle<ShortVFXSpawnRequest>.OnDel(ref ShortVFXSpawnRequest component, short worldID, int entityID) { }
    }
}