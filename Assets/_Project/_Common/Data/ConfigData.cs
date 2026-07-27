using Asteroids.StarshipsFeature;
using Asteroids.BulletsFeature;
using Asteroids.AsteroidsFeature;
using Modules.FX;
using UnityEngine;
using UnityEngine.InputSystem;

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
        public ShortVFXView ShootVFX;
        public float ShootVFXForwardOffset = 0.45f;
    
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

        [Header("Input")]
        public bool ShowMobileControlsOnTouchDevices = true;
        public InputActionReference MoveAction;
        public InputActionReference FireAction;
    }
}
