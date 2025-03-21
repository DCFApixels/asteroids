using Asteroids.Systems;
using TMPro;
using UnityEngine;

namespace Asteroids.Views
{
    public class GameScreen : MonoBehaviour
    {
        public TMP_Text ScoreText;
        public TMP_Text LifeLeftText;

        public GameObject MobileControlRoot;
        public GameButton Left;
        public GameButton Right;
        public GameButton Acceleration;
        public GameButton Shoot;

        public void Show(bool state)
        {
            gameObject.SetActive(state);
        } 
    }
}