using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;

[RequireComponent(typeof(RectTransform))]
public class PopupPanel : MonoBehaviour
{
    [SerializeField] private float duration = 1f;
    [SerializeField] private bool snap = false;
    [SerializeField] private GameObject screenBlocker;
    [SerializeField] private GameObject firstSelection;
    private bool goingDown = false;
    private Tween panelTween;
    private static Tween blockerTween;

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

        EventSystem.current.GetComponent<InputSystemUIInputModule>().enabled = false;

        if (snap)
            GetComponent<RectTransform>().anchoredPosition = Vector2.down * Screen.height;
        screenBlocker.SetActive(true);

        if (blockerTween != null)
            Utils.KillTween(ref blockerTween);
        blockerTween = screenBlocker.GetComponent<Image>().DOFade(0.25f, duration);

        if (panelTween != null)
            Utils.KillTween(ref panelTween);
        panelTween = GetComponent<RectTransform>().DOAnchorPosY(0, duration).SetEase(Ease.OutCubic).OnComplete(() =>
        {
            EventSystem.current.SetSelectedGameObject(firstSelection);
            screenBlocker.GetComponent<Image>().raycastTarget = true;
            action?.Invoke();
            EventSystem.current.GetComponent<InputSystemUIInputModule>().enabled = true;
        });
    }

    public void Down()
    {
        Down(null);
    }

    public void Down(Action action)
    {
        if (goingDown) return;
        goingDown = true;

        if (snap)
            GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

        MainMenuManager.instance.ClosePopup();
        EventSystem.current.GetComponent<InputSystemUIInputModule>().enabled = false;

        screenBlocker.GetComponent<Image>().raycastTarget = false;
        if (blockerTween != null)
            Utils.KillTween(ref blockerTween);
        blockerTween = screenBlocker.GetComponent<Image>().DOFade(0, duration).OnComplete(() =>
        {
            screenBlocker.SetActive(false);
            EventSystem.current.GetComponent<InputSystemUIInputModule>().enabled = true;
        });

        if (panelTween != null)
            Utils.KillTween(ref panelTween);
        panelTween = GetComponent<RectTransform>().DOAnchorPosY(-Screen.height, duration * 2).SetEase(Ease.OutCubic).OnComplete(() =>
        {
            action?.Invoke();
            goingDown = false;
            gameObject.SetActive(false);
        });
    }

    
}
