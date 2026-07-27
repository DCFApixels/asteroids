using DCFApixels.DragonECS;
using Modules.FX;
using UnityEngine;

namespace Asteroids.BulletsFeature
{
    [CreateAssetMenu(menuName = "Asteroids/Features/Bullets Config")]
    public class BulletsFeatureConfig : ScriptableObject, IInjectionUnit
    {
        public BulletDescription ProjectileDescription;
        public float BulletSpeed = 10f;
        public ShortVFXView ShootVFX;
        public float ShootVFXForwardOffset = 0.45f;
        void IInjectionUnit.InitInjectionNode(InjectionGraph graph) { graph.AddNode(this); }
    }
}
