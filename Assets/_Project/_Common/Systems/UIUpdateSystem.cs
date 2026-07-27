using Asteroids.StarshipsFeature;
using DCFApixels.DragonECS;

namespace Asteroids.Systems
{
    internal class UIUpdateSystem : IEcsRun
    {
        [DI] private GameRuntimeData _gameRuntimeData;
        [DI] private StarshipsRuntimeData _starshipsRuntimeData;
        [DI] private GameSceneData _sceneData;

        private int _prevScore = -1;
        private int _prevLives = -1;
        public void Run()
        {
            if (_prevLives != _starshipsRuntimeData.LifeLeft)
            {
                _prevLives = _starshipsRuntimeData.LifeLeft;
                _sceneData.UI.GameScreen.LifeLeftText.text = $"Lives: {_starshipsRuntimeData.LifeLeft}";
            }

            if (_prevScore != _gameRuntimeData.Score)
            {
                _prevScore = _gameRuntimeData.Score;
                _sceneData.UI.GameScreen.ScoreText.text = $"Score: {_gameRuntimeData.Score}";
            }
        }
    }
}
