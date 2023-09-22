using UnityEngine;
using UnityEngine.UI;

public class FramesPerSecond : MonoBehaviour
{
    public float updateInterval = 0.5F;

    private double lastInterval;
    private int frames = 0;
    private float fps;
    public TMPro.TextMeshProUGUI display_Text;
    public KeyCode toggleKey = KeyCode.Tilde;

    //debug test
    // Transform player;
    GameManager gameManager;

    // int totalBackgroundObjects;

    void Awake()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        // player = GameObject.Find("Player").transform;
        // totalBackgroundObjects = gameManager.levelGen.getTotalBackgroundObjects();
        lastInterval = Time.realtimeSinceStartup;
        frames = 0;
    }

    void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            display_Text.enabled = !display_Text.enabled;
        }

        if (display_Text.enabled)
        {
            updateFPS();
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
        // newtext += "Player X: " + player.position.x;
        // newtext += " Total GB Obj: " + totalBackgroundObjects;
        display_Text.text = newtext;
    }
}