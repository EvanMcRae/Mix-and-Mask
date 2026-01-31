using UnityEngine;
using UnityEngine.EventSystems;

public class SelectOnPointerMove : MonoBehaviour, IPointerMoveHandler
{
    public void OnPointerMove(PointerEventData eventData)
    {
        if (EventSystem.current != null && gameObject != null)
            EventSystem.current.SetSelectedGameObject(gameObject);
    }
}
