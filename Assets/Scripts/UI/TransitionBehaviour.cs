using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class TransitionBehaviour : MonoBehaviour
{
    [SerializeField] private Image overlayImage;
    [SerializeField] private float transitionDuration = 0.25f;

    void Awake()
    {
        // Makes sure overlay image is invisible at the start of game
        overlayImage.raycastTarget = false;
        SetAlpha(0f);
    }

    // Generic Fade-In/Out Transition
    public void PlayFade(Action onMidpointSwap)
    {
        StartCoroutine(FadeRoutine(onMidpointSwap));
    }

    // Classic Wipe Fade Transition
    public void PlayWipe(Action onMidpointSwap)
    {
        StartCoroutine(WipeRoutine(onMidpointSwap));
    }

    private IEnumerator FadeRoutine(Action onMidpointSwap)
    {
        overlayImage.raycastTarget = true; // prevents clicks during transitions

        // Fade out start
        yield return AnimateValue(0f, 1f, alpha => SetAlpha(alpha));

        // Swap levels while transition happens
        onMidpointSwap?.Invoke();
        yield return new WaitForSeconds(0.05f);

        // Fade In start
        yield return AnimateValue(1f, 0f, alpha => SetAlpha(alpha));

        overlayImage.raycastTarget = false; // allows clicks
    }

    private IEnumerator WipeRoutine(Action onMidpointSwap)
    {
        overlayImage.raycastTarget = true;
        SetAlpha(1f);

        // Wipe Fade Starts
        overlayImage.fillOrigin = 0;
        yield return AnimateValue(0f, 1f, fill => overlayImage.fillAmount = fill);

        // Swap levels
        onMidpointSwap?.Invoke();
        yield return new WaitForSeconds(0.05f);

        // Wipe away to reveal level
        overlayImage.fillOrigin = 1; // exits to right
        yield return AnimateValue(1f, 0f, fill => overlayImage.fillAmount = fill);

        overlayImage.raycastTarget = false;
    }

    private IEnumerator AnimateValue(float start, float end, Action<float> onUpdate)
    {
        float elapsed = 0f;
        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed /  transitionDuration);

            // This gives it a good ease-in and ease-out
            onUpdate(Mathf.SmoothStep(start, end, t));
            yield return null;
        }
        onUpdate(end);
    }

    private void SetAlpha(float alpha)
    {
        Color c = overlayImage.color;
        c.a = alpha;
        overlayImage.color = c;
    }
}
