using UnityEngine;

public static class FadeCanvas
{

    public static void FadeAlpha(CanvasGroup canvasGroup, float startAlpha, float targetAlpha, float duration, System.Action OnComplete = null)
    {
        if (OnComplete != null)
        {
            LeanTween.alphaCanvas(canvasGroup, targetAlpha, duration).setFrom(startAlpha).setOnComplete(OnComplete);
        }
        else
        {
            LeanTween.alphaCanvas(canvasGroup, targetAlpha, duration).setFrom(startAlpha);
        }
    }

    public static void FadeToOpaque(CanvasGroup canvasGroup, float duration, System.Action onComplete = null)
    {
        //Fade In over 'duration' seconds, target alpha is 1 (completely visible)
        FadeAlpha(canvasGroup, 0f, 1f, duration, onComplete);
    }

    public static void FadeToTransparent(CanvasGroup canvasGroup, float duration, System.Action onComplete = null)
    {
        //Fade Out over 'duration' seconds, target alpha is 0 (completely invisible)
        FadeAlpha(canvasGroup, 1f, 0f, duration, onComplete);
    }
}