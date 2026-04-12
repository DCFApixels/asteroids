using Asteroids.Components;
using DCFApixels.DragonECS;
using System;
using UnityEngine;

namespace Asteroids.Systems
{
    internal class ChangeStateSystem : IEcsRun
    {
        [DI] private SceneData s;
        [DI] private RuntimeData r;
        [DI] private ConfigData c;
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
                if (r.GameState != changeState.NextState)
                {
                    switch (changeState.NextState)
                    {
                        case GameState.Play:
                            _world.GetPool<SpawnStarshipRequest>().NewEntity() = new()
                            {
                                Position = s.SpawnPlayerPosition.position,
                                Rotation = s.SpawnPlayerPosition.rotation,
                            };
                            r.LevelStartTime = Time.time;
                            r.LifeLeft = c.Lifes;
                            r.Score = 0;
                            s.UI.GameScreen.Show(true);
                            s.UI.LoseScreen.Show(false);
                            break;
                        case GameState.Lose:
                            s.UI.GameScreen.Show(false);
                            s.UI.LoseScreen.Show(r.Score);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    r.GameState = changeState.NextState;
                }

                a.ChangeStates.Del(e);
            }
        }
    }
}