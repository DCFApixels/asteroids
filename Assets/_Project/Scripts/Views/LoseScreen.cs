using Asteroids.Components;
using DCFApixels.DragonECS;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Asteroids.Views
{
    public class LoseScreen : MonoBehaviour
    {
        [SerializeField] private TMP_Text ScoreText;
        [SerializeField] private Button RestartButton;
        private EcsDefaultWorld _world;

        public void InjectWorld(EcsDefaultWorld world)
        {
            _world = world;
        }

        private void Start()
        {
            RestartButton.onClick.AddListener(OnRestartButtonClicked);
        }

        private void OnDestroy()
        {
            RestartButton.onClick.RemoveListener(OnRestartButtonClicked);
        }

        private void OnRestartButtonClicked()
        {
            _world.GetPool<RestartEvent>().Add(_world.NewEntity());
        }

        public void Show(int score)
        {
             ScoreText.text = $"Your score: {score}";
             Show(true);
        }

        public void Show(bool state)
        {
            gameObject.SetActive(state);
        }
    }
}