using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;

public class ScreenTransition : MonoBehaviour
{
    public static ScreenTransition instance; // not really a singleton but just so we can have some convenience
    public MaskEffectTransition met;
    public static bool inProgress = false;

    void Awake()
    {
        met = GetComponent<MaskEffectTransition>();
        instance = this;
        In();
    }

    public static void In(Action action = null)
    {
        inProgress = true;
        instance.met.StartTransition(false, action);
    }

    public static void Out(Action action = null)
    {
        inProgress = true;
        EventSystem.current.GetComponent<InputSystemUIInputModule>().enabled = false;
        instance.met.StartTransition(true, action);
    }
}
