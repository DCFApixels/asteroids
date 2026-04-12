using DCFApixels.DragonECS;

namespace Asteroids.LocalInputFeature
{
    [MetaGroup(LocalInputModule.META_GROUP)]
    [MetaColor(LocalInputModule.META_COLOR)]
    [System.Serializable]
    public struct LocalInputReceiver : IEcsComponent { }
}
