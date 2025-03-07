using UnityEngine;

namespace Asteroids.Views
{
    public class BulletView : MonoBehaviour
    {
        public float Radius = 0.5f;

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position, Radius);
        }
    }
}