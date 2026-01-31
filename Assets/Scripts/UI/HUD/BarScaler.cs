using DG.Tweening;
using UnityEngine;
using UnityEngine.UIElements;

public class BarScaler : MonoBehaviour
{
    public Image eyeImage;
    //Image Transforms
    [SerializeField] public RectTransform leftBar;
    [SerializeField] private RectTransform rightBar;

    [Header("COPY FROM BAR TRANSFORMS")]
    [SerializeField] private float maxWidth = 384;
    [SerializeField] private float tweenDuration = 0.35f;
    [SerializeField] private Ease ease = Ease.OutCubic;

    private Tween leftTween;
    private Tween rightTween;

    public void UpdateBars(float ratio)
    {
        //ensure 0-1
        ratio = Mathf.Clamp01(ratio);
        //target
        float targetWidth = ratio * maxWidth;

        Vector2 leftSize = leftBar.sizeDelta;
        Vector2 rightSize = rightBar.sizeDelta;

        leftSize.x = targetWidth;
        rightSize.x = targetWidth;

        //exit out of tween early if there
        //exit out of tween early if there
        leftTween?.Kill();
        rightTween?.Kill();

        //Ease in the tween animation
        leftTween = leftBar.DOSizeDelta(leftSize, tweenDuration).SetEase(ease);
        rightTween = rightBar.DOSizeDelta(rightSize, tweenDuration).SetEase(ease);

    }

    //test func
    public void RandomizeBars()
    {
        float randomRatio = Random.Range(0f, 1f);
        UpdateBars(randomRatio);
    }
}
