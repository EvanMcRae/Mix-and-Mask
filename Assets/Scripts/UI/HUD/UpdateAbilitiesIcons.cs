using DG.Tweening;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;
using static ControllableEnemy;
public class UpdateAbilitiesIcons : MonoBehaviour
{
    private const float FULL_WIDTH = 344f;

    [SerializeField] Image rightImageA;
    [SerializeField] Image rightImageB;

    [SerializeField] Image fillA;
    [SerializeField] Image fillB;

    //test Sprites
    [SerializeField] Sprite[] iconSprites;

    //test variable
    // int testIndex = 0;

    //Animating the fall animation
    [SerializeField] private CanvasGroup canvasGroup;

    //animation params
    [SerializeField] float fallDistance = 300f;
    [SerializeField] float fallDuration = 0.5f;
    [SerializeField] float rotateAmount = 25f;

    [Header("Slot-In Settings")]
    [SerializeField] float slideDistance = 600f;
    [SerializeField] float slideDuration = 0.45f;
    [SerializeField] float overshoot = 20f;

    //Ability right stuff
    private RectTransform rect;
    private Vector2 startPos;
    private Quaternion startRot;

    //AbilityCenter
    [SerializeField] RectTransform centerRect;
    private Vector2 centerStartPos;
    private Quaternion centerStartRot;

    [SerializeField] GameObject canvasParent;

    //Needed for connecting to mask logic
    DetatchedMask maskController;

    private bool isDead = false;


    //Set the things needs to animate the canvas
    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        startPos = rect.anchoredPosition;
        startRot = rect.localRotation;
        centerStartPos = centerRect.anchoredPosition;
        centerStartRot = centerRect.localRotation;
        maskController = FindFirstObjectByType<DetatchedMask>();

    }


    //Call when possession ends
    public void UIFallOff()
    {
        rect.DOKill();
        canvasGroup.DOKill();

        Sequence seq = DOTween.Sequence();
        //Move the UI down
        seq.Join(rect.DOAnchorPosY(startPos.y - fallDistance,fallDuration).SetEase(Ease.InQuad));
        //Add the rotate
        seq.Join(rect.DORotate(new Vector3(0, 0, Random.Range(0, rotateAmount)),fallDuration));
        //Fade out
        seq.Join(canvasGroup.DOFade(0f, fallDuration * 0.8f));
        //Hide the game object
        seq.OnComplete(() =>{canvasParent.SetActive(false);});
    }

    public void UICenterFallOff()
    {
        centerRect.DOKill();

        Sequence seq = DOTween.Sequence();
        //Move the UI down
        seq.Join(centerRect.DOAnchorPosY(centerStartPos.y - fallDistance, fallDuration).SetEase(Ease.InQuad));
        //Add the rotate
        seq.Join(centerRect.DORotate(new Vector3(0, 0, Random.Range(0, rotateAmount)), fallDuration));
        //Fade out
        seq.Join(centerRect.GetComponent<Image>().DOFade(0f, fallDuration * 0.8f));
        //Hide the game object
        seq.OnComplete(() => { centerRect.gameObject.SetActive(false); });
    }

    public void UISlotIn()
    {
        canvasParent.SetActive(true);

        rect.DOKill();
        canvasGroup.DOKill();

        // Start off-screen to the right
        rect.anchoredPosition = startPos + Vector2.left * slideDistance;
        rect.localRotation = startRot;
        canvasGroup.alpha = 0f;

        Sequence seq = DOTween.Sequence();

        // Slide past slot slightly
        seq.Append(rect.DOAnchorPos(startPos + Vector2.right * overshoot, slideDuration * 0.75f).SetEase(Ease.OutCubic));
        // Settle into slot
        seq.Append(rect.DOAnchorPos(startPos,slideDuration * 0.25f).SetEase(Ease.OutQuad));
        //Fade in the UI
        seq.Join(canvasGroup.DOFade(1f, slideDuration * 0.6f));
    }

    //private void AnimateCooldownRect(Image fillImage, float startWidth, float cooldownDuration)
    //{
    //    RectTransform rt = fillImage.rectTransform;
    //    rt.DOKill();

    //    // Reset fill
    //    rt.sizeDelta = new Vector2(startWidth, rt.sizeDelta.y);

    //    // POP animation
    //    AnimateCooldownStart(fillImage);

    //    // Drain cooldown
    //    rt.DOSizeDelta(
    //        new Vector2(0f, rt.sizeDelta.y),
    //        cooldownDuration
    //    )
    //    .SetEase(Ease.Linear)
    //    .SetLink(fillImage.gameObject, LinkBehaviour.KillOnDisable);
    //}


    public void AnimateCooldownRect(Image image, float cooldownDuration)
    {
        if (image == null) return;

        image.DOKill();

        // Reset fill to full
        image.fillAmount = 1f;

        // Animate fillAmount from 1 to 0 over cooldownDuration
        image.DOFillAmount(0f, cooldownDuration)
             .SetEase(Ease.Linear)
             .SetLink(image.gameObject, LinkBehaviour.KillOnDisable);
    }

    private void AnimateCooldownStart(RectTransform mask)
    {
        RectTransform rt = mask;

        rt.DOKill();

        // Reset scale
        rt.localScale = Vector3.one * 0.85f;

        rt.DOScale(1f, 0.2f)
          .SetEase(Ease.OutBack)
          .SetLink(mask.gameObject, LinkBehaviour.KillOnDisable);
    }

    public void UpdateAbilityIcons(EnemyType type)
    {
        int firstIndex = 0;
        int secondIndex = 0;

        //Could be a scriptable object but im not doing all that
        switch (type)
        {
            case EnemyType.Runner:
                firstIndex = 0;
                secondIndex = 1;
                break;
            case EnemyType.Tank:
                firstIndex = 2;
                secondIndex = 3;
                break;
            case EnemyType.None:
            //make the UI fall away
            default:
                break;
        }

        //reset the icons

        //Don't update if youre not on the mask
        if (type != EnemyType.None)
        {
            rightImageA.sprite = iconSprites[firstIndex];

            rightImageB.sprite = iconSprites[secondIndex];
        }

        UISlotIn();

    }

    public void ResetAbilitiesUI()
    {
        rect.anchoredPosition = startPos;
        rect.localRotation = startRot;
        canvasGroup.alpha = 1f;
        gameObject.SetActive(true);

        UpdateAbilityIcons(EnemyType.Runner);
        
    }

    public void OnAttach()
    {
        DetatchedMask maskController = FindFirstObjectByType<DetatchedMask>();

        if (maskController == null)
        {
            Debug.Log("NO DETATCHED MASK TO PULL TYPE FROM");
            return;
        }

        //Update the UI based on what you did
        UpdateAbilityIcons(maskController.GetCurrentlyControlledEnemyType());
    }

    public void OnDettach()
    {
        

        if (maskController == null)
        {
            Debug.Log("NO DETATCHED MASK TO PULL TYPE FROM");
            return;
        }

        //Update the UI based on what you did
        UIFallOff();
    }

    public void CoolDownSecondary(float cooldown)
    {
        if (!isDead)
        {

            AnimateCooldownRect(fillB, cooldown);
        }
    }

    public void CoolDownPrimary(float cooldown)
    {
        if (!isDead)
        {
            AnimateCooldownRect(fillA, cooldown);
        }
    }

    public void OnPlayerDie()
    {
        UICenterFallOff();
        UIFallOff();
        isDead = true;
    }


    public void OnEnable()
    {
        if (maskController != null)
        {
            maskController.onAttach.AddListener(OnAttach);
            maskController.onDetach.AddListener(OnDettach);
        }
    }

    public void OnDisable()
    {
        if (maskController != null)
        {
            maskController.onAttach.RemoveListener(OnAttach);
            maskController.onDetach.RemoveListener(OnDettach);
        }
    }
}
