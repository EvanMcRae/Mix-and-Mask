using UnityEngine;
using UnityEngine.EventSystems;

public class SelectOnPointerMove : MonoBehaviour, IPointerMoveHandler, IPointerEnterHandler
{
    public static bool disabledEnter = false;

    public void OnPointerMove(PointerEventData eventData)
    {
        if (EventSystem.current != null && gameObject != null)
            EventSystem.current.SetSelectedGameObject(gameObject);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (EventSystem.current != null && gameObject != null && !disabledEnter)
            EventSystem.current.SetSelectedGameObject(gameObject);
    }
}
