using Asteroids.Views;
using DCFApixels.DragonECS;
using UnityEngine;

namespace Asteroids.Data
{
    [CreateAssetMenu]
    internal class StaticData : ScriptableObject
    {
        [Header("Player")]
        public ScriptableEntityTemplateBase PlayerStarshipTemplate;
        public StarshipView StarshipViewPrefab;
        public float RotationSpeed = 10;
        public float StarshipSpeed = 10;
        public float StarshipAcceleration = 10;
        public float StarshipMaxSpeed;
        public int Lifes = 3;
        public float StarshipSpawnImmunityTime = 1;
        [Range(0,1)]
        public float StarshipFriction = 0.01f;
    
        [Header("Bullets")]
        public ScriptableEntityTemplateBase BulletTemplate;
        public BulletView BulletViewPrefab;
        public float BulletSpeed = 10;
    
        [Header("Asteroid")]
        public ScriptableEntityTemplateBase AsteroidTemplate;
        public AsteroidView AsteroidViewPrefab;
        public int SpawnFrequency = 3;
        public int SpawnAmount = 10;
        public float AsteroidMaxSpeed = 1;
        public float AsteroidMinSpeed = 5;
        public int AsteroidDeathLeft = 2;
        [Range(1.1f, 2)]
        public float AdditionalKillOffset = 1.1f;

        public AsteroidExplosion AsteroidExplosion;

    }
}