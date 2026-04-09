using UnityEngine;

namespace Asteroids.Views
{
    public class ProjectileView : ViewBase
    {
        public float Radius = 0.5f;

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position, Radius);
        }
    }
}