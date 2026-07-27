using Asteroids.StarshipsFeature;
using DCFApixels.DragonECS;

namespace Asteroids.Systems
{
    internal class UIUpdateSystem : IEcsRun
    {
        [DI] GameRuntimeData _gameRuntime;
        [DI] StarshipsRuntimeData _starshipsRuntime;
        [DI] GameSceneData _sceneData;

        private int _prevScore = -1;
        private int _prevLives = -1;
        public void Run()
        {
            if (_prevLives != _starshipsRuntime.LifeLeft)
            {
                _prevLives = _starshipsRuntime.LifeLeft;
                _sceneData.UI.GameScreen.LifeLeftText.text = $"Lives: {_starshipsRuntime.LifeLeft}";
            }

            if (_prevScore != _gameRuntime.Score)
            {
                _prevScore = _gameRuntime.Score;
                _sceneData.UI.GameScreen.ScoreText.text = $"Score: {_gameRuntime.Score}";
            }
        }
    }
}
