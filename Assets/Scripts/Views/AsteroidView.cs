using UnityEngine;

namespace Asteroids.Views
{
    internal class AsteroidView : MonoBehaviour
    {
        public float Radius = 2;

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position, Radius);
        }
    }
}