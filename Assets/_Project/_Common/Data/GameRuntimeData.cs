namespace Asteroids
{
    internal enum GameState
    {
        None = 0,
        Play,
        Lose
    }

    internal class GameRuntimeData
    {
        public GameState GameState;
        public int Score;
    }
}
