using Asteroids.GameFieldFeature;
using Asteroids.LocalInputFeature;
using DCFApixels.DragonECS;
using UnityEngine;

namespace Asteroids
{
    [CreateAssetMenu]
    internal class ConfigData : ScriptableObject, IInjectionBlock
    {
        [Header("Module Configs")]
        public LocalInputModuleConfig LocalInput;
        public GameFieldModuleConfig GameField;

        [Header("Feature Configs")]
        public ScriptableObject[] FeatureConfigs;

        public void InjectTo(Injector inj)
        {
            inj.Inject(new GameRuntimeData());

            InjectIfNotNull(inj, LocalInput);
            InjectIfNotNull(inj, GameField);

            if (FeatureConfigs == null)
            {
                return;
            }

            foreach (ScriptableObject featureConfig in FeatureConfigs)
            {
                InjectIfNotNull(inj, featureConfig);
            }
        }

        private static void InjectIfNotNull<T>(Injector inj, T data)
            where T : Object
        {
            if (data != null)
            {
                inj.Inject(data);
            }
        }
    }
}
