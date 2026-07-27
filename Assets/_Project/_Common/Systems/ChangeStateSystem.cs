using Asteroids.AsteroidsFeature;
using Asteroids.StarshipsFeature;
using Asteroids.Components;
using DCFApixels.DragonECS;
using System;
using UnityEngine;

namespace Asteroids.Systems
{
    internal class ChangeStateSystem : IEcsRun
    {
        [DI] private GameSceneData s;
        [DI] private StarshipsFeatureSceneData starshipsSceneData;
        [DI] private GameRuntimeData gameRuntimeData;
        [DI] private StarshipsRuntimeData starshipsRuntimeData;
        [DI] private AsteroidsRuntimeData asteroidsRuntimeData;
        [DI] private StarshipsFeatureConfig c;
        [DI] private EcsDefaultWorld _world;

        private class Aspect : EcsAspect
        {
            public readonly EcsPool<ChangeState> ChangeStates = Inc;
        }
        
        public void Run()
        {
            foreach (var e in _world.Where(out Aspect a))
            {
                ref var changeState = ref a.ChangeStates.Get(e);
                if (gameRuntimeData.GameState != changeState.NextState)
                {
                    switch (changeState.NextState)
                    {
                        case GameState.Play:
                            _world.GetPool<SpawnStarshipRequest>().NewEntity() = new()
                            {
                                Position = starshipsSceneData.SpawnPlayerPosition.position,
                                Rotation = starshipsSceneData.SpawnPlayerPosition.rotation,
                            };
                            asteroidsRuntimeData.LevelStartTime = Time.time;
                            starshipsRuntimeData.LifeLeft = c.Lifes;
                            gameRuntimeData.Score = 0;
                            s.UI.GameScreen.Show(true);
                            s.UI.LoseScreen.Show(false);
                            break;
                        case GameState.Lose:
                            s.UI.GameScreen.Show(false);
                            s.UI.LoseScreen.Show(gameRuntimeData.Score);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    gameRuntimeData.GameState = changeState.NextState;
                }

                a.ChangeStates.Del(e);
            }
        }
    }
}
