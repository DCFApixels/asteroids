using Asteroids.Views;
using DCFApixels.DragonECS;

namespace Asteroids.Components
{
    public struct Bullet : IEcsComponent
    {
        public BulletView View;
    }
}