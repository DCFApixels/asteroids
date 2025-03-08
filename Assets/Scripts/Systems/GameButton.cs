using UnityEngine;
using UnityEngine.EventSystems;

namespace Asteroids.Systems
{
    public class GameButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
    {
        [SerializeField] private bool isClickButton;
    
        public bool IsDown;
    
        public void OnPointerDown(PointerEventData eventData)
        {
            if (!isClickButton)
            {
                IsDown = true;
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!isClickButton)
            {
                IsDown = false;
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (isClickButton)
            {
                IsDown = true;
            }
        }
    }
}