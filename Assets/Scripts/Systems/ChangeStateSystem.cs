using System;
using Asteroids.Components;
using Asteroids.Data;
using DCFApixels.DragonECS;
using UnityEngine;

namespace Asteroids.Systems
{
    class ChangeStateSystem : IEcsRun
    {
        [DI] private SceneData _sceneData;
        [DI] private RuntimeData _runtimeData;
        [DI] private EcsWorld _world;
        [DI] private StaticData _staticData;
        class Aspect : EcsAspect
        {
            public EcsPool<ChangeState> ChangeStates = Inc;
        }
        
        public void Run()
        {
            foreach (var e in _world.Where(out Aspect a))
            {
                ref var changeState = ref a.ChangeStates.Get(e);
                if (_runtimeData.GameState != changeState.NewState)
                {
                    switch (changeState.NewState)
                    {
                        case GameState.Play:
                            _world.GetPool<SpawnStarshipEvent>().Add(_world.NewEntity());
                            _runtimeData.LevelStartTime = Time.time;
                            _runtimeData.LifeLeft = _staticData.Lifes;
                            _runtimeData.Score = 0;
                            _sceneData.UI.GameScreen.Show(true);
                            _sceneData.UI.LoseScreen.Show(false);
                            break;
                        case GameState.Lose:
                            _sceneData.UI.GameScreen.Show(false);
                            _sceneData.UI.LoseScreen.Show(_runtimeData.Score);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    _runtimeData.GameState = changeState.NewState;
                }

                a.ChangeStates.Del(e);
            }
        }
    }
}