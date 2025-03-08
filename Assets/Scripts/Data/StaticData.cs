using Asteroids.Views;
using UnityEngine;
using UnityEngine.Serialization;

namespace Asteroids.Data
{
    [CreateAssetMenu]
    internal class StaticData : ScriptableObject
    {
        public StarshipView StarshipView;
        public float RotationSpeed = 10;
        public float StarshipSpeed = 10;
        public float StarshipAcceleration = 10;
        public float StarshipMaxSpeed;
        public int Lifes = 3;
        public float StarshipSpawnImmunityTime = 1;
        [Range(0,1)]
        public float StarshipFriction = 0.01f;
    
        [Header("Bullets")]
        public BulletView BulletView;
        public float BulletSpeed = 10;
    
        [Header("Asteroid")]
        public AsteroidView AsteroidView;
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