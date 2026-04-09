using Asteroids.Views;
using UnityEngine;

namespace Asteroids
{
    internal class SceneData : MonoBehaviour
    {
        public Transform SpawnPlayerPosition;
        public Camera Camera;
        public UI UI;
        public float KillOnSpawnRadius = 5;
    }
}