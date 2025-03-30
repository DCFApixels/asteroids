using Asteroids.Data;
using DCFApixels.DragonECS;
using System.Security;

namespace Asteroids.Components
{
    [System.Serializable]
    internal struct ChangeState : IEcsComponent
    {
        public GameState NextState;
    }
}