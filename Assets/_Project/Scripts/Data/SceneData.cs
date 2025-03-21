using Asteroids.Views;
using UnityEngine;

namespace Asteroids.Data
{
    internal class SceneData : MonoBehaviour
    {
        public Transform SpawnPosition;
        public float KillOnSpawnRadius = 5;
        public Camera Camera;
        public UI UI;
    }
}