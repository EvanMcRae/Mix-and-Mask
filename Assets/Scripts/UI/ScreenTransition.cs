using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;

[RequireComponent(typeof(Image))]
public class ScreenTransition : MonoBehaviour
{
    public static ScreenTransition instance; // not really a singleton but just so we can have some convenience
    private readonly float duration = 0.5f;
    private Image image;

    void Awake()
    {
        image = GetComponent<Image>();
        instance = this;
        In();
    }

    public static void In()
    {
        In(null);
    }

    public static void In(Action action)
    {
        EventSystem.current.GetComponent<InputSystemUIInputModule>().enabled = false;
        instance.image.raycastTarget = true;

        // TODO: @dax do your shader magic
        instance.image.DOFade(1, 0).Complete();
        instance.image.DOFade(0, instance.duration).OnComplete(() =>
        {
            EventSystem.current.GetComponent<InputSystemUIInputModule>().enabled = true;
            instance.image.raycastTarget = false;
            action?.Invoke();
        });
    }

    public static void Out()
    {
        Out(null);
    }

    public static void Out(Action action)
    {
        EventSystem.current.GetComponent<InputSystemUIInputModule>().enabled = false;
        instance.image.raycastTarget = true;

        // TODO: @dax do your shader magic
        instance.image.DOFade(0, 0).Complete();
        instance.image.DOFade(1, instance.duration).OnComplete(() =>
        {
            action?.Invoke();
        });
    }
}
