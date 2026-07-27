using DCFApixels.DragonECS;
using UnityEngine;

namespace Asteroids.StarshipsFeature
{
    [CreateAssetMenu(menuName = "Asteroids/Features/Starships Config")]
    public class StarshipsFeatureConfig : ScriptableObject, IInjectionUnit, IInjectionBlock
    {
        public StarshipDescription PlayerStarshipTemplate;
        public int Lifes = 3;
        public float StarshipSpawnImmunityTime = 1f;

        public void InjectTo(Injector inj)
        {
            inj.Inject(new StarshipsRuntimeData());
        }

        void IInjectionUnit.InitInjectionNode(InjectionGraph graph) { graph.AddNode(this); }
    }
}
