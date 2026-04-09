using DCFApixels.DragonECS;
using Modules.BoundsOverlaps;
using Modules.CameraController;
using UnityEngine;

namespace Asteroids
{
    internal enum GameState
    {
        None = 0,
        Play,
        Lose
    }
    [System.Serializable]
    internal class RuntimeData : IInjectionBlock
    {
        public GameState GameState;
        public int LifeLeft;
        public int Score;
        public Vector2 FieldSize;
        public float LevelStartTime;

        public BoundsOverlapsRuntime BoundsOverlapsRuntime;
        public CameraBrain CameraBrain;
        public void InjectTo(Injector inj)
        {
            inj.Inject(BoundsOverlapsRuntime);
        }
    }
}