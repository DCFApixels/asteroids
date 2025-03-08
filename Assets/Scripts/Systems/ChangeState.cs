using Asteroids.Data;
using DCFApixels.DragonECS;

namespace Asteroids.Systems
{
    struct ChangeState : IEcsComponent
    {
        public GameState NewState;
    }
}