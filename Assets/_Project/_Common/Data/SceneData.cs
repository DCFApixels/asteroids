using Asteroids.GameFieldFeature;
using DCFApixels.DragonECS;
using UnityEngine;

namespace Asteroids
{
    internal class SceneData : MonoBehaviour, IInjectionBlock
    {
        [Header("Game Scene Data")]
        public GameSceneData Game;

        [Header("Module Scene Data")]
        public GameFieldModuleSceneData GameField;

        [Header("Feature Scene Data")]
        public FeatureSceneData[] FeatureSceneData;

        public void InjectTo(Injector inj)
        {
            inj.Inject(Game);
            inj.Inject(GameField);
            if (GameField.Camera != null)
            {
                inj.Inject(GameField.Camera);
            }

            if (FeatureSceneData == null) { return; }

            foreach (FeatureSceneData featureSceneData in FeatureSceneData)
            {
                inj.Inject(featureSceneData);
            }
        }
    }
}
