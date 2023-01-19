using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Crosswork.Demo
{
    public enum InputEventType
    {
        BeginDrag,
        Drag,
        EndDrag,
        Click,
        DoubleClick
    }

    public delegate void InputHandler(InputEventType type, PointerEventData data);

    [RequireComponent(typeof(Graphic))]
    public class DemoBoardInput : MonoBehaviour, IPointerClickHandler, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        public event InputHandler Input;

        private Graphic panel;

        private void Awake()
        {
            panel = GetComponent<Graphic>();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Input?.Invoke(InputEventType.Click, eventData);

            if (eventData.clickCount == 2)
            {
                Input?.Invoke(InputEventType.DoubleClick, eventData);
            }
        }

        public void SetActive(bool state)
        {
            panel.raycastTarget = state;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            Input?.Invoke(InputEventType.BeginDrag, eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            Input?.Invoke(InputEventType.Drag, eventData);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            Input?.Invoke(InputEventType.EndDrag, eventData);
        }
    }
}
