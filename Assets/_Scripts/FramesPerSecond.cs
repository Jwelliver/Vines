using UnityEngine;
using UnityEngine.UI;

public class FramesPerSecond : MonoBehaviour
{
    public float updateInterval = 0.5F;
    private double lastInterval;
    private int frames = 0;
    private float fps;
    static TMPro.TextMeshProUGUI display_Text;
    static bool ShowFPS;

    void Awake()
    {
        lastInterval = Time.realtimeSinceStartup;
        frames = 0;
        display_Text = GetComponent<TMPro.TextMeshProUGUI>();
    }

    public static void OnFpsButtonPressed()
    {
        ShowFPS = !ShowFPS;
        display_Text.enabled = ShowFPS;
    }

    void Update()
    {
        if (ShowFPS) { UpdateFPS(); }
    }

    void UpdateFPS()
    {
        ++frames;
        float timeNow = Time.realtimeSinceStartup;

        if (timeNow > lastInterval + updateInterval)
        {
            fps = (float)(frames / (timeNow - lastInterval));
            frames = 0;
            lastInterval = timeNow;
            string frameTime = (Time.smoothDeltaTime * 1000).ToString("F2");
            display_Text.text = Mathf.Ceil(fps).ToString() + " FPS (" + frameTime + "ms)";
        }
    }
}