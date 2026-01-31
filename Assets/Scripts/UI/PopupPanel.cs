using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.InputSystem;

[RequireComponent(typeof(RectTransform))]
public class PopupPanel : MonoBehaviour
{
    [SerializeField] private float duration = 1f;
    [SerializeField] private bool snap = false;
    [SerializeField] private GameObject screenBlocker;
    private bool goingDown = false;
    private Tween blockerTween, panelTween;

    void Update()
    {
        if (Keyboard.current[Key.Escape].wasPressedThisFrame)
        {
            Down();
        }
    }

    public void Up(Action action = null)
    {
        goingDown = false;
        gameObject.SetActive(true);

        if (snap)
            GetComponent<RectTransform>().anchoredPosition = Vector2.down * Screen.height;
        screenBlocker.SetActive(true);

        if (blockerTween != null)
            KillTween(ref blockerTween);
        blockerTween = screenBlocker.GetComponent<Image>().DOFade(0.25f, duration);

        if (panelTween != null)
            KillTween(ref panelTween);
        panelTween = GetComponent<RectTransform>().DOAnchorPosY(0, duration).SetEase(Ease.OutCubic).OnComplete(() =>
        {
            action?.Invoke();
        });
    }

    public void Down(Action action = null)
    {
        if (goingDown) return;
        goingDown = true;

        if (snap)
            GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

        if (blockerTween != null)
            KillTween(ref blockerTween);
        blockerTween = screenBlocker.GetComponent<Image>().DOFade(0, duration).OnComplete(() =>
        {
            screenBlocker.SetActive(false);
        });

        if (panelTween != null)
            KillTween(ref panelTween);
        panelTween = GetComponent<RectTransform>().DOAnchorPosY(-Screen.height, duration * 2).SetEase(Ease.OutCubic).OnComplete(() =>
        {
            action?.Invoke();
            goingDown = false;
            gameObject.SetActive(false);
        });
    }

    public void KillTween(ref Tween currTween)
    {
        if (currTween != null && currTween.IsActive() && !currTween.IsComplete())
        {
            currTween.Kill();
            currTween = null;
        }
    }
}
