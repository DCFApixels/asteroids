using Asteroids.Views;
using DCFApixels.DragonECS;

namespace Asteroids.StarshipsFeature
{
    [MetaGroup(StarshipsModule.META_GROUP, EcsConsts.COMPONENTS_GROUP)]
    [MetaColor(StarshipsModule.META_COLOR)]
    [System.Serializable]
    public struct Starship : IEcsComponent
    {
        public StarshipDescription Description;
        public StarshipView View;
    }
}
