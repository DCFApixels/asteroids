using DCFApixels.DragonECS;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Asteroids.Data
{
    public class LoseScreen : MonoBehaviour
    {
        [SerializeField] private TMP_Text ScoreText;
        [SerializeField] private Button RestartButton;
        private EcsWorld _world;

        public void InjectWorld(EcsWorld world)
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