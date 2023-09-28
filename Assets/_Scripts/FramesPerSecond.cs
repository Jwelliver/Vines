using UnityEngine;
using UnityEngine.UI;

public class FramesPerSecond : MonoBehaviour
{
    public float updateInterval = 0.5F;
    private double lastInterval;
    private int frames = 0;
    private float fps;
    public TMPro.TextMeshProUGUI display_Text;
    public static bool ShowFPS;
    // public KeyCode toggleKey = KeyCode.BackQuote;


    void Awake()
    {
        lastInterval = Time.realtimeSinceStartup;
        frames = 0;
    }

    public static void OnFpsButtonPressed()
    {
        ShowFPS = !ShowFPS;
    }

    void Update() //TODO: need a nicer way of handling this (text enabled) instead of on Udpate
    {
        if (ShowFPS)
        {
            if (!display_Text.enabled) { display_Text.enabled = true; }
            updateFPS();
        }
        else
        {
            if (display_Text.enabled) { display_Text.enabled = false; }
        }
    }

    void updateFPS()
    {
        ++frames;
        float timeNow = Time.realtimeSinceStartup;
        if (timeNow > lastInterval + updateInterval)
        {
            fps = (float)(frames / (timeNow - lastInterval));
            frames = 0;
            lastInterval = timeNow;
        }

        string newtext = "FPS: " + Mathf.Ceil(fps).ToString();
        display_Text.text = newtext;
    }
}