using Asteroids.Views;
using DCFApixels.DragonECS;

namespace Asteroids.Components
{
    internal struct Starship : IEcsComponent
    {
        public StarshipView View;
    }
}