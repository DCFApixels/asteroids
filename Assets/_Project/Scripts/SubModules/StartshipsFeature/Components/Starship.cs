using Asteroids.Views;
using DCFApixels.DragonECS;

namespace Asteroids.StartshipsFeature
{
    [System.Serializable]
    public struct Starship : IEcsComponent
    {
        public StarshipDescription Description;
        public StarshipView View;
    }
}