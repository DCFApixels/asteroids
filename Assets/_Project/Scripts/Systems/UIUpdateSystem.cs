using Asteroids.Data;
using DCFApixels.DragonECS;

namespace Asteroids.Systems
{
    internal class UIUpdateSystem : IEcsRun
    {
        [DI] private RuntimeData _runtimeData;
        [DI] private SceneData _sceneData;

        private int _prevScore = -1;
        private int _prevLives = -1;
        public void Run()
        {
            if (_prevLives != _runtimeData.LifeLeft)
            {
                _prevLives = _runtimeData.LifeLeft;
                _sceneData.UI.GameScreen.LifeLeftText.text = $"Lives: {_runtimeData.LifeLeft}";
            }

            if (_prevScore != _runtimeData.Score)
            {
                _prevScore = _runtimeData.Score;
                _sceneData.UI.GameScreen.ScoreText.text = $"Score: {_runtimeData.Score}";
            }
        }
    }
}