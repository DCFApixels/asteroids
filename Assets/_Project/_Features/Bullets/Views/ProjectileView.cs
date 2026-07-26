using Asteroids.Views;
using UnityEngine;

namespace Asteroids.BulletsFeature
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