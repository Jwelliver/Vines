using System.Collections;
using UnityEngine;
using TMPro;
using System;

public class FadeText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_TextComponent;
    [SerializeField] private float FadeSpeed = 20.0f;
    [SerializeField] private int RolloverCharacterSpread = 10;
    [SerializeField] private bool fadeInOnStart = false;
    [SerializeField] private bool fadeOutAfterComplete = false;
    [SerializeField] private float secondsBeforeFadeOut = 1f; //nSeconds after Fade in complete before beginning fade out.
    [SerializeField] private bool disableAfterFadeOut = true;

    enum FadeMode
    {
        FadeIn,
        FadeOut
    }


    private WaitForSeconds waitForOneSecond;
    private WaitForSeconds waitForSecondsPerChar;

    // Temp action caches; Will be stored when received in FadeTo()
    private Action FadeInCompleteAction;
    private Action FadeOutCompleteAction;

    void Start()
    {
        // Init
        waitForSecondsPerChar = new WaitForSeconds(0.25f - FadeSpeed * 0.01f);
        // If FadeInOnStart, begin init fade in
        if (fadeInOnStart) { StartCoroutine(Fade(FadeMode.FadeIn)); }
    }

    public void FadeTo(string text = null, Action OnFadeInComplete = null, Action OnFadeOutComplete = null)
    {
        if (OnFadeInComplete != null) { FadeInCompleteAction = OnFadeInComplete; }
        if (OnFadeInComplete != null && !fadeOutAfterComplete)
        {
            Debug.LogError("Warning: FadeOutComplete callback provided, but text is not set to Fade Out.");
        }
        else
        {
            FadeOutCompleteAction = OnFadeOutComplete;
        }
        if (!m_TextComponent.enabled) m_TextComponent.enabled = true;
        //Update the text if provided; Otherwise use the textMeshGui's preset text.
        if (text != null) { m_TextComponent.text = text; }
        StartCoroutine(Fade(FadeMode.FadeIn));
    }

    void OnFadeComplete(FadeMode fadeMode)
    {
        switch (fadeMode)
        {
            case FadeMode.FadeIn:
                {
                    // Handle FadeInComplete callback
                    if (FadeInCompleteAction != null)
                    {
                        FadeInCompleteAction.Invoke();
                        FadeInCompleteAction = null;
                    }
                    // Handle fade out if enabled
                    if (fadeOutAfterComplete)
                    {
                        StartCoroutine(Fade(FadeMode.FadeOut, secondsBeforeFadeOut));
                    }
                    break;
                }
            case FadeMode.FadeOut:
                {
                    //Handle Disable after fade out
                    if (disableAfterFadeOut) { m_TextComponent.enabled = false; }
                    //Handle FadeOutComplete callback
                    if (FadeOutCompleteAction != null)
                    {
                        FadeOutCompleteAction.Invoke();
                        FadeInCompleteAction = null;
                    }
                    break;
                }
        }
    }


    // TODO: Fade can probably be static and placed elsewhere, then FadeText monobehavior can be a FadeTextController that implements it
    /// <summary>
    /// Method to animate vertex colors of a TMP Text object.
    /// </summary>
    /// <returns></returns>
    IEnumerator Fade(FadeMode fadeMode, float initialDelay = 0f)
    {
        int startAlpha = fadeMode == FadeMode.FadeIn ? 0 : 255;
        int targetAlpha = fadeMode == FadeMode.FadeIn ? 255 : 0;

        // Handle InitialDelay
        yield return new WaitForSeconds(initialDelay);

        // Set the whole text transparent
        m_TextComponent.color = new Color
            (
                m_TextComponent.color.r,
                m_TextComponent.color.g,
                m_TextComponent.color.b,
                startAlpha
            );
        // Need to force the text object to be generated so we have valid data to work with right from the start.
        m_TextComponent.ForceMeshUpdate();

        TMP_TextInfo textInfo = m_TextComponent.textInfo;
        Color32[] newVertexColors;

        int currentCharacter = 0;
        int startingCharacterRange = currentCharacter;
        bool isRangeMax = false;

        int characterCount = textInfo.characterCount;

        while (!isRangeMax)
        {
            // Spread should not exceed the number of characters.
            byte fadeSteps = (byte)Mathf.Max(1, 255 / RolloverCharacterSpread);

            //If we're fading out; then negate fadeSteps
            // if (fadeMode == FadeMode.FadeOut) fadeSteps = (byte)-fadeSteps;

            for (int i = startingCharacterRange; i < currentCharacter + 1; i++)
            {
                // Skip characters that are not visible (like white spaces)
                if (!textInfo.characterInfo[i].isVisible) continue;

                // Get the index of the material used by the current character.
                int materialIndex = textInfo.characterInfo[i].materialReferenceIndex;

                // Get the vertex colors of the mesh used by this text element (character or sprite).
                newVertexColors = textInfo.meshInfo[materialIndex].colors32;

                // Get the index of the first vertex used by this text element.
                int vertexIndex = textInfo.characterInfo[i].vertexIndex;

                // Get the current character's alpha value.
                byte alpha;
                if (fadeMode == FadeMode.FadeIn)
                {
                    alpha = (byte)Mathf.Clamp(newVertexColors[vertexIndex].a + fadeSteps, 0, 255);
                }
                else
                {
                    alpha = (byte)Mathf.Clamp(newVertexColors[vertexIndex].a - fadeSteps, 0, 255);
                }


                // Set new alpha values.
                newVertexColors[vertexIndex + 0].a = alpha;
                newVertexColors[vertexIndex + 1].a = alpha;
                newVertexColors[vertexIndex + 2].a = alpha;
                newVertexColors[vertexIndex + 3].a = alpha;

                if (alpha == targetAlpha)
                {
                    startingCharacterRange += 1;

                    // When FadeIn is Complete
                    if (startingCharacterRange == characterCount)
                    {
                        // Update mesh vertex data one last time.
                        m_TextComponent.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);

                        yield return waitForOneSecond;

                        // Call OnFadeComplete
                        OnFadeComplete(fadeMode);

                        // Exit Coroutine
                        yield break;
                    }
                }
            }

            // Upload the changed vertex colors to the Mesh.
            m_TextComponent.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);

            if (currentCharacter + 1 < characterCount) currentCharacter += 1;

            yield return waitForSecondsPerChar;
        }
    }
}