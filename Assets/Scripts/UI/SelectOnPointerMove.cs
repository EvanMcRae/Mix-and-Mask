using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectOnPointerMove : MonoBehaviour, IPointerMoveHandler, IPointerEnterHandler, ISelectHandler, IDeselectHandler, IPointerDownHandler, IPointerUpHandler
{
    public static bool disabledEnter = false;
    private bool selected = false, pressed = false;
    [SerializeField] private GameObject[] dependencies;

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

    public void OnSelect(BaseEventData eventData)
    {
        selected = true;
        foreach (GameObject g in dependencies)
        {
            if (g.TryGetComponent(out TextMeshProUGUI text))
            {
                if (!pressed)
                    text.color = GetComponent<Selectable>().colors.selectedColor;
            }
            else if (g.TryGetComponent(out Image image))
            {
                if (!pressed)
                    image.color = GetComponent<Selectable>().colors.selectedColor;
            }
        }
    }

    public void OnDeselect(BaseEventData eventData)
    {
        selected = false;
        foreach (GameObject g in dependencies)
        {
            if (g.TryGetComponent(out TextMeshProUGUI text))
            {
                if (!pressed)
                    text.color = GetComponent<Selectable>().colors.normalColor;
            }
            else if (g.TryGetComponent(out Image image))
            {
                if (!pressed)
                    image.color = GetComponent<Selectable>().colors.normalColor;
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        pressed = true;
        foreach (GameObject g in dependencies)
        {
            if (g.TryGetComponent(out TextMeshProUGUI text))
            {
                text.color = GetComponent<Selectable>().colors.pressedColor;
            }
            else if (g.TryGetComponent(out Image image))
            {
                image.color = GetComponent<Selectable>().colors.pressedColor;
            }
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        pressed = false;
        foreach (GameObject g in dependencies)
        {
            if (g.TryGetComponent(out TextMeshProUGUI text))
            {
                text.color = selected ? GetComponent<Selectable>().colors.selectedColor : GetComponent<Selectable>().colors.normalColor;
            }
            else if (g.TryGetComponent(out Image image))
            {
                image.color = selected ? GetComponent<Selectable>().colors.selectedColor : GetComponent<Selectable>().colors.normalColor;
            }
        }
    }
}
