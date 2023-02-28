using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class FadeText : MonoBehaviour
{

    [Tooltip("Number of seconds each character should take to fade up")]
    public float fadeDuration = 2f;

    [Tooltip("Speed the reveal travels along the text, in characters per second")]
    public float travelSpeed = 8f;

    // Cached reference to our Text object.
    TMPro.TMP_Text _text;

    Coroutine _fade;

    // Lookup table for hex characters.
    static readonly char[] NIBBLE_TO_HEX = new char[] {
        '0', '1', '2', '3',
        '4', '5', '6', '7',
        '8', '9', 'A', 'B',
        'C', 'D', 'E', 'F'};

    // Use this for initialization
    void Start () {
        _text = GetComponent<TMPro.TMP_Text>();

        // If you don't want the text to fade right away, skip this line.
        FadeTo(_text.text);
        _text.text="";
    }

    public void FadeTo(string text)
    {
         // Abort a fade in progress, if any.
         StopFade();

         // Start fading, and keep track of the coroutine so we can interrupt if needed.
         _fade = StartCoroutine(fadeText(text));
    }

    public void StopFade() { 
         if(_fade != null)
               StopCoroutine(_fade);
    }

    // Currently this expects a string of plain text,
    // and will not correctly handle rich text tags etc.
    IEnumerator fadeText(string text) {

        int length = text.Length;

        // Build a character buffer of our desired text,
        // with a rich text "color" tag around every character.
        var builder = new System.Text.StringBuilder(length * 26);    
        Color32 color = _text.color;            
        for(int i = 0; i < length; i++) {
            builder.Append("<color=#");
            builder.Append(NIBBLE_TO_HEX[color.r >> 4]);
            builder.Append(NIBBLE_TO_HEX[color.r & 0xF]);
            builder.Append(NIBBLE_TO_HEX[color.g >> 4]);
            builder.Append(NIBBLE_TO_HEX[color.g & 0xF]);
            builder.Append(NIBBLE_TO_HEX[color.b >> 4]);
            builder.Append(NIBBLE_TO_HEX[color.b & 0xF]);
            builder.Append("00>");
            builder.Append(text[i]);
            builder.Append("</color>");
        }

        // Each frame, update the alpha values along the fading frontier.
        float fadingProgress = 0f;
        int opaqueChars = -1;    
        while(opaqueChars < length - 1) { 
            yield return null;

            fadingProgress += Time.deltaTime;

            float leadingEdge = fadingProgress * travelSpeed;

            int lastChar = Mathf.Min(length - 1, Mathf.FloorToInt(leadingEdge));

            int newOpaque = opaqueChars;

            for(int i = lastChar; i > opaqueChars; i--) {
                byte fade = (byte)(255f * Mathf.Clamp01((leadingEdge - i)/(travelSpeed * fadeDuration)));
                builder[i * 26 + 14] = NIBBLE_TO_HEX[fade >> 4];
                builder[i * 26 + 15] = NIBBLE_TO_HEX[fade & 0xF];

                if (fade == 255)
                    newOpaque = Mathf.Max(newOpaque, i);
            }

            opaqueChars = newOpaque;

            // This allocates a new string.
            _text.text = builder.ToString();
        }

        // Once all the characters are opaque, 
        // ditch the unnecessary markup and end the routine.
        _text.text = text;

        // Mark the fade transition as finished.
        // This can also fire an event/message if you want to signal UI.
        _fade = null;
    }
}
