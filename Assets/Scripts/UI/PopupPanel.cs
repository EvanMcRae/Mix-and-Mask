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
    [SerializeField] private float blockerOpacity = 0.25f;
    [SerializeField] private bool snap = false;
    [SerializeField] private GameObject screenBlocker;
    [SerializeField] private GameObject firstSelection;
    private GameObject previousObject;
    private bool goingDown = false;
    [SerializeField] private bool closesOnEscape = true;
    private Tween panelTween, separateBlockerTween;
    private static Tween globalBlockerTween;
    [SerializeField] private bool overlayPanel = false;
    private bool isUp = false;
    public static bool overlayUp = false;
    public Action OnClose;

    void Start()
    {
        overlayUp = false;
    }

    void Update()
    {
        if (isUp && closesOnEscape && Keyboard.current[Utils.IsWebPlayer() ? Key.Tab : Key.Escape].wasPressedThisFrame)
        {
            Down();
        }
    }

    public void Up(Action action = null)
    {
        isUp = true;
        goingDown = false;
        gameObject.SetActive(true);

        previousObject = EventSystem.current.currentSelectedGameObject;
        EventSystem.current.GetComponent<InputSystemUIInputModule>().enabled = false;
        if (snap)
            GetComponent<RectTransform>().anchoredPosition = Vector2.down * Screen.height;
        screenBlocker.SetActive(true);

        // I am so fucking sorry for this code
        if (!overlayPanel)
        {
            if (globalBlockerTween != null)
                Utils.KillTween(ref globalBlockerTween);
            globalBlockerTween = screenBlocker.GetComponent<Image>().DOFade(blockerOpacity, duration).SetUpdate(true);
        }
        else
        {
            overlayUp = true;
            if (separateBlockerTween != null)
                Utils.KillTween(ref separateBlockerTween);
            separateBlockerTween = screenBlocker.GetComponent<Image>().DOFade(blockerOpacity, duration).SetUpdate(true);
        }

        if (panelTween != null)
            Utils.KillTween(ref panelTween);
        panelTween = GetComponent<RectTransform>().DOAnchorPosY(0, duration).SetEase(Ease.OutCubic).SetUpdate(true).OnComplete(() =>
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

        OnClose?.Invoke();

        AkUnitySoundEngine.PostEvent("Back", Utils.WwiseGlobal);

        if (snap)
            GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        
        SelectOnPointerMove.disabledEnter = true;
        SelectOnPointerMove.disabledSound = true;
        EventSystem.current.SetSelectedGameObject(previousObject);
        EventSystem.current.GetComponent<InputSystemUIInputModule>().enabled = false;

        screenBlocker.GetComponent<Image>().raycastTarget = false;

        // I am so fucking sorry for this code
        if (!overlayPanel)
        {
            if (globalBlockerTween != null)
                Utils.KillTween(ref globalBlockerTween);
            globalBlockerTween = screenBlocker.GetComponent<Image>().DOFade(0, duration).SetUpdate(true).OnComplete(() =>
            {
                screenBlocker.SetActive(false);
                EventSystem.current.GetComponent<InputSystemUIInputModule>().enabled = true;
                isUp = false;
                SelectOnPointerMove.disabledSound = false;
                Invoke(nameof(EnablePointerEnter), 0.1f);
            });
        }
        else
        {
            if (separateBlockerTween != null)
                Utils.KillTween(ref separateBlockerTween);
            separateBlockerTween = screenBlocker.GetComponent<Image>().DOFade(0, duration).SetUpdate(true).OnComplete(() =>
            {
                screenBlocker.SetActive(false);
                EventSystem.current.GetComponent<InputSystemUIInputModule>().enabled = true;
                isUp = false;
                overlayUp = false;
                SelectOnPointerMove.disabledSound = false;
                Invoke(nameof(EnablePointerEnter), 0.1f);
            });
        }
        
        if (panelTween != null)
            Utils.KillTween(ref panelTween);
        panelTween = GetComponent<RectTransform>().DOAnchorPosY(-1080, duration * 2).SetEase(Ease.OutCubic).SetUpdate(true).OnComplete(() =>
        {
            action?.Invoke();
            goingDown = false;
        });
    }

    public void EnablePointerEnter()
    {
        SelectOnPointerMove.disabledEnter = false;
    }
}
