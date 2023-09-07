using UnityEngine;
using UnityEngine.UI;

public class FramesPerSecond : MonoBehaviour
{
    public float updateInterval = 0.5F;

    private double lastInterval;
    private int frames = 0;
    private float fps;
    public TMPro.TextMeshProUGUI display_Text;
    public KeyCode toggleKey = KeyCode.F1;

    void Start()
    {
        lastInterval = Time.realtimeSinceStartup;
        frames = 0;
    }

    void Update()
    {
        ++frames;
        float timeNow = Time.realtimeSinceStartup;
        if (timeNow > lastInterval + updateInterval)
        {
            fps = (float)(frames / (timeNow - lastInterval));
            frames = 0;
            lastInterval = timeNow;
        }

        display_Text.text = "FPS: " + Mathf.Ceil(fps).ToString();

        if (Input.GetKeyDown(toggleKey))
        {
            display_Text.enabled = !display_Text.enabled;
        }
    }
}