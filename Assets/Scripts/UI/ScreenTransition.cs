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

    void Awake()
    {
        met = GetComponent<MaskEffectTransition>();
        instance = this;
        In();
    }

    public static void In(Action action = null)
    {
        instance.met.StartTransition(false, action);
    }

    public static void Out(Action action = null)
    {
        instance.met.StartTransition(true, action);
    }
}
