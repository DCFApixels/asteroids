using Asteroids.Views;
using DCFApixels.DragonECS;
using Modules.CameraController;
using UnityEngine;

namespace Asteroids
{
    internal class SceneData : MonoBehaviour, IInjectionBlock
    {
        public Transform SpawnPlayerPosition;
        public CameraBrain Camera;
        public UI UI;
        public float KillOnSpawnRadius = 5;

        public void InjectTo(Injector inj)
        {
            inj.Inject(Camera);
        }
    }
}