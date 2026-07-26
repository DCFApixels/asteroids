using Asteroids.StarshipsFeature;
using Asteroids.BulletsFeature;
using Asteroids.AsteroidsFeature;
using DCFApixels.DragonECS;
using DCFApixels.DragonECS.Unity;
using DCFApixels.DragonECS.Unity.Attributes;
using Modules.BoundsOverlaps;
using UnityEngine;

namespace Asteroids
{
    [CreateAssetMenu]
    internal class ConfigData : ScriptableObject
    {
        [Header("Player")]
        public StarshipDescription PlayerStarshipTemplate;
        public int Lifes = 3;
        public float StarshipSpawnImmunityTime = 1;

    
        [Header("Bullets")]
        public BulletDescription ProjectileDescription;
        public float BulletSpeed = 10;
    
        [Header("Asteroid")]
        public AsteroidDescription AsteroidDescription;
        public int SpawnFrequency = 3;
        public int SpawnAmount = 10;
        public float AsteroidMaxSpeed = 1;
        public float AsteroidMinSpeed = 5;
        public float AsteroidSplitMultiplier = 0.70f;
        [Range(1.1f, 2)]
        public float AdditionalKillOffset = 1.1f;

        [Header("Other")]
        public float ScreenBorderOffset = 0.5f;


        [DragonMetaBlock]
        [ReferenceDropDown(typeof(IEcsModule))]
        [SerializeReference]
        public IEcsModule[] Features;



        [DragonMetaBlock]
        [ReferenceDropDown(typeof(IEcsModule))]
        public BoundsOverlapsModule[] Features_2;
    }
}