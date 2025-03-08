using System;
using Asteroids.Utils;
using UnityEngine;

namespace Asteroids.Views
{
    internal class AsteroidExplosion : MonoBehaviour
    {
        public ParticleSystem Explosion;
        public float Time;

        private void Awake()
        {
            Time = Explosion.main.duration;
        }

        public async Awaitable Play(PoolService poolService, int poolId)
        {
            gameObject.SetActive(true);
            Explosion.Play();
            await Awaitable.WaitForSecondsAsync(Time);
            poolService.Return(poolId, this);
        }
    }
}