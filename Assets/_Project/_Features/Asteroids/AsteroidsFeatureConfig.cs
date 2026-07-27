using DCFApixels.DragonECS;
using UnityEngine;

namespace Asteroids.AsteroidsFeature
{
    [CreateAssetMenu(menuName = "Asteroids/Features/Asteroids Config")]
    public class AsteroidsFeatureConfig : ScriptableObject, IInjectionUnit, IInjectionBlock
    {
        public AsteroidDescription AsteroidDescription;
        public int SpawnFrequency = 3;
        public int SpawnAmount = 10;
        public float AsteroidMaxSpeed = 1f;
        public float AsteroidMinSpeed = 5f;
        public float AsteroidSplitMultiplier = 0.70f;

        public void InjectTo(Injector inj)
        {
            inj.Inject(new AsteroidsRuntimeData());
        }

        void IInjectionUnit.InitInjectionNode(InjectionGraph graph) { graph.AddNode(this); }
    }
}
