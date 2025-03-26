using Asteroids.Views;
using DCFApixels.DragonECS;

namespace Asteroids.Components
{
    [System.Serializable]
    public struct Starship : IEcsComponent
    {
        public StarshipView View;
    }
}