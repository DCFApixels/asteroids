using Asteroids.Data;
using DCFApixels.DragonECS;

namespace Asteroids.Components
{
    internal struct ChangeState : IEcsComponent
    {
        public GameState NewState;
    }
}