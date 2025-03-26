using System;
using Asteroids.Utils;
using UnityEngine;

namespace Asteroids.Views
{
    internal class ExplosionView : MonoBehaviour
    {
        public ParticleSystem Explosion;
        public float Time;

        private void Awake()
        {
            Time = Explosion.main.duration;
        }

        public async void Play(PoolService poolService, int poolId)
        {
            try
            {
                gameObject.SetActive(true);
                Explosion.Play();
                await Awaitable.WaitForSecondsAsync(Time);
                poolService.Return(poolId, this);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
    }
}