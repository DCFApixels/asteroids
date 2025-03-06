using UnityEngine;

namespace Asteroids.Views
{
    internal class StarshipView : MonoBehaviour
    {
        public float Radius;

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position, Radius);
        }
    }
}