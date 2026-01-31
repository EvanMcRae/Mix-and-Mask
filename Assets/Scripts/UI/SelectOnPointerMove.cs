using UnityEngine;
using UnityEngine.EventSystems;

public class SelectOnPointerMove : MonoBehaviour, IPointerMoveHandler
{
    public void OnPointerMove(PointerEventData eventData)
    {
        EventSystem.current.SetSelectedGameObject(gameObject);
    }
}
