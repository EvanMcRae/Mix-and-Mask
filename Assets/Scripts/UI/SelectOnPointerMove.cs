using UnityEngine;
using UnityEngine.EventSystems;

public class SelectOnPointerMove : MonoBehaviour, IPointerMoveHandler, IPointerEnterHandler
{
    public void OnPointerMove(PointerEventData eventData)
    {
        if (EventSystem.current != null && gameObject != null)
            EventSystem.current.SetSelectedGameObject(gameObject);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (EventSystem.current != null && gameObject != null)
            EventSystem.current.SetSelectedGameObject(gameObject);
    }
}
