using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;

public class MaskEffectTransition : MonoBehaviour {


    public float speed = 1;
    public bool transitionDirection = false;
    public bool isTransitioning = true;
    public float t = 1;

    public Action onComplete;

    // True direction is closing in, false is closing out from in.
    public void StartTransition(bool direction, Action onCompleteCallback) {
        transitionDirection = direction;
        isTransitioning = true;
        if (onComplete != null) onComplete.Invoke();
        onComplete = onCompleteCallback;
    }

    public void Update() {
        if (!isTransitioning) return;
        t += (transitionDirection ? 1 : -1) * speed * Time.deltaTime;
        t = Mathf.Clamp(t, 0, 1);
        if (isTransitioning) TransitionRendererFeature.instance.ApplyTransition(t);
        if (Mathf.Approximately(t, 1f) || t == 0f) {
            isTransitioning = false;
            if (onComplete != null) onComplete.Invoke();
            ScreenTransition.inProgress = false;
            EventSystem.current.GetComponent<InputSystemUIInputModule>().enabled = true;
        }
    }
    
    
}
