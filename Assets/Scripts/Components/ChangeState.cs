using Asteroids.Data;
using DCFApixels.DragonECS;

namespace Asteroids.Components
{
    struct ChangeState : IEcsComponent
    {
        public GameState NewState;
    }
}