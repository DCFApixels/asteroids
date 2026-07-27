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
        [DI] GameSceneData _sceneData;
        [DI] StarshipsFeatureSceneData _starshipsSceneData;
        [DI] GameRuntimeData _gameRuntime;
        [DI] StarshipsRuntimeData _starshipsRuntime;
        [DI] AsteroidsRuntimeData _asteroidsRuntime;
        [DI] StarshipsFeatureConfig _starshipsConfig;
        [DI] EcsDefaultWorld _world;

        class Aspect : EcsAspect
        {
            public readonly EcsPool<ChangeState> ChangeStates = Inc;
        }
        
        public void Run()
        {
            foreach (var e in _world.Where(out Aspect a))
            {
                ref var changeState = ref a.ChangeStates.Get(e);
                if (_gameRuntime.GameState != changeState.NextState)
                {
                    switch (changeState.NextState)
                    {
                        case GameState.Play:
                            _world.GetPool<SpawnStarshipRequest>().NewEntity() = new()
                            {
                                Position = _starshipsSceneData.SpawnPlayerPosition.position,
                                Rotation = _starshipsSceneData.SpawnPlayerPosition.rotation,
                            };
                            _asteroidsRuntime.LevelStartTime = Time.time;
                            _starshipsRuntime.LifeLeft = _starshipsConfig.Lifes;
                            _gameRuntime.Score = 0;
                            _sceneData.UI.GameScreen.Show(true);
                            _sceneData.UI.LoseScreen.Show(false);
                            break;
                        case GameState.Lose:
                            _sceneData.UI.GameScreen.Show(false);
                            _sceneData.UI.LoseScreen.Show(_gameRuntime.Score);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    _gameRuntime.GameState = changeState.NextState;
                }

                a.ChangeStates.Del(e);
            }
        }
    }
}
