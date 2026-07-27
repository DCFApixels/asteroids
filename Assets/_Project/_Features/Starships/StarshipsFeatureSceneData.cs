using UnityEngine;
using DCFApixels.DragonECS;

namespace Asteroids.StarshipsFeature
{
    public class StarshipsFeatureSceneData : FeatureSceneData, IInjectionUnit
    {
        public Transform SpawnPlayerPosition;
        public float KillOnSpawnRadius = 5f;
        void IInjectionUnit.InitInjectionNode(InjectionGraph graph) { graph.AddNode(this); }
    }
}
